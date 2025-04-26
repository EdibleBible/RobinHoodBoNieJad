using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class NewMapGenerator : MonoBehaviour
{
    private Unity.Mathematics.Random random;


    public CustomGrid.Grid<GridCellData> CreatedGrid;
    public GridParameters GridParameters;
    public RoomGeneratorSettings RoomGeneratorSettings;


    public Dictionary<GridCellData, GameObject> allObject = new Dictionary<GridCellData, GameObject>();
    [SerializeField] private SOLevel levelData;

    [Header("Pathfinding")] public DelaunayTriangulator Triangulator;
    public List<Point> AllPoints = new List<Point>();
    public List<Edge> AllEdges = new List<Edge>();
    public List<Edge> SelectedEdges = new List<Edge>();
    public List<Triangle> AllTriangles = new List<Triangle>();
    public List<GridCellData> Hallwaycell = new List<GridCellData>();
    public bool AvaibleDifferentRoomsOnly;

    private void Awake()
    {
        if (GridParameters.IsRandomized)
        {
            if (GridParameters.RandomizeSeed)
                RandimizeSeed();

            GenerateGrid(levelData.LevelSeedUint);
        }
        else
        {
            GenerateGrid(GridParameters.GridSize.x, GridParameters.GridSize.y);
        }

        random = new Unity.Mathematics.Random(levelData.LevelSeedUint);

        RoomGeneratorSettings.CreateRoomOnGrid(CreatedGrid, levelData);
        
        DebugGridMesh();

        GenerateTriangles(CreatedGrid.GetAllGridElementList(), AvaibleDifferentRoomsOnly);

        DebugGridMesh();

        
        SelectedEdges = GetUsedEdgesPointVersion(AllEdges, AllPoints);

        DebugGridMesh();
    }

    public void RandimizeSeed()
    {
        var randimInt = new System.Random().Next(1000, 9999);
        levelData.LevelSeedInt = randimInt;
        levelData.LevelSeedUint = (uint)randimInt;
    }

    public void GenerateGrid(uint seed)
    {
        GridParameters.RandomizeGridSize(seed);
        GenerateGrid(GridParameters.GridSize.x, GridParameters.GridSize.y);
    }

    public void GenerateGrid(int gridX, int gridY)
    {
        // Tworzenie głównej siatki
        CreatedGrid = new CustomGrid.Grid<GridCellData>(gridX, gridY, GridParameters.CellSize, transform.position,
            (CustomGrid.Grid<GridCellData> g, int x, int y) =>
            {
                GridCellData cellData = new GridCellData();
                cellData.GridCellType = E_GridCellType.Empty;
                cellData.SetCoordinate(x, y);
                cellData.SetCellSize(GridParameters.CellSize);

                // Obliczanie pozycji (start od 0, 0)
                Vector3 position = new Vector3(x * GridParameters.CellSize, 0, y * GridParameters.CellSize);
                cellData.SetPosition(position);
                return cellData;
            });

        foreach (var gridElement in CreatedGrid.GetGridArray())
        {
            gridElement.SetGridParent(CreatedGrid);
        }
    }

    public void DebugGridMesh()
    {
        if (CreatedGrid == null)
            return;

        Vector2Int gridSize = CreatedGrid.GeneratedGridSize();
        var coppy = allObject;

        foreach (var cellData in coppy)
        {
            DestroyImmediate(allObject.Where(x => x.Key == cellData.Key).FirstOrDefault().Value);
        }

        allObject.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        foreach (var gridCell in CreatedGrid.GetGridArray())
        {
            var cell = gridCell;
            if (cell == null) continue;
            if (GridParameters.materials.TryGetValue(cell.GridCellType, out var material))
            {
                var createdCell = new GameObject($"cell nr x:{cell.Coordinate.x} y:{cell.Coordinate.y}");
                createdCell.transform.parent = transform;

                createdCell.transform.position = cell.Position +
                                                 new Vector3(GridParameters.CellSize, 0, GridParameters.CellSize) *
                                                 0.5f;

                createdCell.transform.localScale = new Vector3(GridParameters.CellSize, GridParameters.CellSize,
                    GridParameters.CellSize);

                MeshFilter meshFilter = createdCell.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

                MeshRenderer meshRenderer = createdCell.AddComponent<MeshRenderer>();
                meshRenderer.material = material;

                if (createdCell == null)
                    continue;
                // Skalowanie
                createdCell.transform.localScale = Vector3.one * GridParameters.CellSize;
                allObject.Add(cell, createdCell);
            }
        }
    }

    public List<Triangle> GenerateTriangulation()
    {
        Triangulator = new DelaunayTriangulator();
        Dictionary<NewRoom, Point> listOfPoints = new Dictionary<NewRoom, Point>();

        // Tworzenie punktów
        foreach (NewRoom roomCentre in RoomGeneratorSettings.AllCreatedRooms)
        {
            Point newPoint = new Point(roomCentre.Centroid.x, roomCentre.Centroid.z);
            newPoint.SetPointRoom(roomCentre);
            listOfPoints.Add(roomCentre, newPoint);
        }

        // Generowanie triangulacji
        List<Triangle> allTriangle = Triangulator.BowyerWatson(listOfPoints).ToList();


        AllEdges = GetEdgesFromTriangles(allTriangle);
        AllPoints = GetPointsFromTriangles(allTriangle);
        return allTriangle;
    }

    public void GenerateTriangles(List<GridCellData> allCells, bool avaibleDifferentRoomsOnly = false)
    {
        AllTriangles = new List<Triangle>();

        List<GridCellData> passCells = allCells
            .Where(cell => cell.GridCellType == E_GridCellType.Pass)
            .ToList();

        // 2. Tworzymy mapę GridCellData -> Point
        Dictionary<GridCellData, Point> pointLookup = new Dictionary<GridCellData, Point>();
        foreach (var cell in passCells)
        {
            float scaledX = cell.Coordinate.x * CreatedGrid.GetCellSize();
            float scaledY = cell.Coordinate.y * CreatedGrid.GetCellSize();
            Point point = new Point(scaledX, scaledY);
            pointLookup[cell] = point;
        }

        // 3. Tworzymy wszystkie możliwe trójkąty (kombinacje po 3 punkty)
        for (int i = 0; i < passCells.Count; i++)
        {
            for (int j = i + 1; j < passCells.Count; j++)
            {
                for (int k = j + 1; k < passCells.Count; k++)
                {
                    var cell1 = passCells[i];
                    var cell2 = passCells[j];
                    var cell3 = passCells[k];

                    // SPRAWDZAMY: jeśli przynajmniej dwa punkty są w tym samym pomieszczeniu => pomijamy
                    if (avaibleDifferentRoomsOnly)
                    {
                        bool sameRoom =
                            (cell1.RoomID == cell2.RoomID) ||
                            (cell1.RoomID == cell3.RoomID) ||
                            (cell2.RoomID == cell3.RoomID);
                        if (sameRoom)
                        {
                            Debug.Log($"Same room: {cell1.RoomID}, {cell2.RoomID}, {cell3.RoomID}");
                            continue; // pomiń tworzenie trójkąta
                        }
                    }

                    var p1 = pointLookup[cell1];
                    var p2 = pointLookup[cell2];
                    var p3 = pointLookup[cell3];

                    try
                    {
                        Triangle triangle = new Triangle(p1, p2, p3);
                        AllTriangles.Add(triangle);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"Nie udało się stworzyć trójkąta: {e.Message}");
                    }
                }
            }
        }

        Debug.LogWarning($"triangles: {AllTriangles.Count}");

        AllEdges = GetEdgesFromTriangles(AllTriangles);
        AllPoints = GetPointsFromTriangles(AllTriangles);
    }


    public List<Edge> GetEdgesFromTriangles(List<Triangle> triangles)
    {
        List<Edge> edges = new List<Edge>();

        foreach (var triangle in triangles)
        {
            // Dodaj krawędzie do listy
            AddEdgeIfNotExist(edges, new Edge(triangle.Vertices[0], triangle.Vertices[1]));
            AddEdgeIfNotExist(edges, new Edge(triangle.Vertices[1], triangle.Vertices[2]));
            AddEdgeIfNotExist(edges, new Edge(triangle.Vertices[2], triangle.Vertices[0]));
        }

        return edges;
    }

    private void AddEdgeIfNotExist(List<Edge> edges, Edge edge)
    {
        // Dodaj krawędź do listy, jeśli jeszcze jej tam nie ma (uwzględniając odwrotne kierunki)
        if (!edges.Contains(edge))
        {
            edges.Add(edge);
        }
    }

    public List<Point> GetPointsFromTriangles(List<Triangle> triangles)
    {
        List<Point> points = new List<Point>();

        foreach (var triangle in triangles)
        {
            // Dodaj punkty wierzchołków trójkąta do listy, jeśli jeszcze ich tam nie ma
            AddPointIfNotExist(points, triangle.Vertices[0]);
            AddPointIfNotExist(points, triangle.Vertices[1]);
            AddPointIfNotExist(points, triangle.Vertices[2]);
        }

        return points;
    }

    private void AddPointIfNotExist(List<Point> points, Point point)
    {
        // Dodaj punkt do listy, jeśli jeszcze go tam nie ma
        if (!points.Contains(point))
        {
            points.Add(point);
        }
    }

    public List<Edge> GetUsedEdgesRoomVersion(List<Edge> allEdge, List<Point> allPoints)
    {
        List<Edge> usedEdge = PrimAlgorithm.FindMST(allEdge, allPoints);
        int additionalEdges = 0;
        HashSet<Vector2Int> mergedPoints = new HashSet<Vector2Int>();
        if (!RoomGeneratorSettings.UseAdditionalEdges)
        {
            foreach (Edge edge in usedEdge)
            {
                var edgePoint1Vector2 = new Vector2((float)edge.Point1.X, (float)edge.Point1.Y);
                var edgePoint2Vector2 = new Vector2((float)edge.Point2.X, (float)edge.Point2.Y);

                var room1 = RoomGeneratorSettings.AllCreatedRooms
                    .FirstOrDefault(x => x.Centroid.x == edgePoint1Vector2.x && x.Centroid.z == edgePoint1Vector2.y);
                var room2 = RoomGeneratorSettings.AllCreatedRooms
                    .FirstOrDefault(x => x.Centroid.x == edgePoint2Vector2.x && x.Centroid.z == edgePoint2Vector2.y);

                var connectionPoints = FindConnectionPosition(room1, room2);

                // Sprawdzanie i scalanie punktów wejścia/wyjścia
                Vector2Int entryPointCoordinate = connectionPoints.entryPoint.Coordinate;
                Vector2Int exitPointCoordinate = connectionPoints.exitPoint.Coordinate;

                if (IsPointMerged(entryPointCoordinate, mergedPoints))
                {
                    entryPointCoordinate = GetClosestMergedPoint(entryPointCoordinate, mergedPoints);
                }
                else
                {
                    mergedPoints.Add(entryPointCoordinate);
                }

                if (IsPointMerged(exitPointCoordinate, mergedPoints))
                {
                    exitPointCoordinate = GetClosestMergedPoint(exitPointCoordinate, mergedPoints);
                }
                else
                {
                    mergedPoints.Add(exitPointCoordinate);
                }

                edge.SetEdgeRoom(room1, room2);

                GridCellData enterCellData = CreatedGrid.GetValue(entryPointCoordinate.x, entryPointCoordinate.y);
                GridCellData exitCellData = CreatedGrid.GetValue(exitPointCoordinate.x, exitPointCoordinate.y);

                edge.SetEnterExitRoom(enterCellData, exitCellData);
            }

            return usedEdge;
        }

        foreach (Edge edge in allEdge)
        {
            if (additionalEdges >= RoomGeneratorSettings.AdditionSelectedEdges)
                break;

            if (usedEdge.Contains(edge))
                continue;

            var randomNumber = random.NextInt(0, 101);

            if (randomNumber > RoomGeneratorSettings.ChanceToSelectEdge)
                continue;

            usedEdge.Add(edge);
            additionalEdges++;
        }

        foreach (Edge edge in usedEdge)
        {
            var edgePoint1Vector2 = new Vector2((float)edge.Point1.X, (float)edge.Point1.Y);
            var edgePoint2Vector2 = new Vector2((float)edge.Point2.X, (float)edge.Point2.Y);

            var room1 = RoomGeneratorSettings.AllCreatedRooms
                .FirstOrDefault(x => x.Centroid.x == edgePoint1Vector2.x && x.Centroid.z == edgePoint1Vector2.y);
            var room2 = RoomGeneratorSettings.AllCreatedRooms
                .FirstOrDefault(x => x.Centroid.x == edgePoint2Vector2.x && x.Centroid.z == edgePoint2Vector2.y);

            var connectionPoints = FindConnectionPosition(room1, room2);

            // Sprawdzanie i scalanie punktów wejścia/wyjścia
            Vector2Int entryPointCoordinate = connectionPoints.entryPoint.Coordinate;
            Vector2Int exitPointCoordinate = connectionPoints.exitPoint.Coordinate;

            if (IsPointMerged(entryPointCoordinate, mergedPoints))
            {
                entryPointCoordinate = GetClosestMergedPoint(entryPointCoordinate, mergedPoints);
            }
            else
            {
                mergedPoints.Add(entryPointCoordinate);
            }

            if (IsPointMerged(exitPointCoordinate, mergedPoints))
            {
                exitPointCoordinate = GetClosestMergedPoint(exitPointCoordinate, mergedPoints);
            }
            else
            {
                mergedPoints.Add(exitPointCoordinate);
            }

            edge.SetEdgeRoom(room1, room2);

            GridCellData enterCellData = CreatedGrid.GetValue(entryPointCoordinate.x, entryPointCoordinate.y);
            GridCellData exitCellData = CreatedGrid.GetValue(exitPointCoordinate.x, exitPointCoordinate.y);

            edge.SetEnterExitRoom(enterCellData, exitCellData);
        }

        return usedEdge;
    }

    public List<Edge> GetUsedEdgesPointVersion(List<Edge> allEdge, List<Point> allPoints)
    {
        List<Edge> usedEdge = PrimAlgorithm.FindMST(allEdge, allPoints);
        int additionalEdges = 0;
        HashSet<Vector2Int> mergedPoints = new HashSet<Vector2Int>();
        if (!RoomGeneratorSettings.UseAdditionalEdges)
        {
            foreach (Edge edge in usedEdge)
            {
                var edgePoint1Vector2 = new Vector2((float)edge.Point1.X, (float)edge.Point1.Y);
                var edgePoint2Vector2 = new Vector2((float)edge.Point2.X, (float)edge.Point2.Y);

                var connectionPoints = FindConnectionPosition(edgePoint1Vector2, edgePoint2Vector2);

                // Sprawdzanie i scalanie punktów wejścia/wyjścia
                Vector2Int entryPointCoordinate = connectionPoints.entryPoint.Coordinate;
                Vector2Int exitPointCoordinate = connectionPoints.exitPoint.Coordinate;

                if (IsPointMerged(entryPointCoordinate, mergedPoints))
                {
                    entryPointCoordinate = GetClosestMergedPoint(entryPointCoordinate, mergedPoints);
                }
                else
                {
                    mergedPoints.Add(entryPointCoordinate);
                }

                if (IsPointMerged(exitPointCoordinate, mergedPoints))
                {
                    exitPointCoordinate = GetClosestMergedPoint(exitPointCoordinate, mergedPoints);
                }
                else
                {
                    mergedPoints.Add(exitPointCoordinate);
                }
                
                GridCellData enterCellData = CreatedGrid.GetValue(entryPointCoordinate.x, entryPointCoordinate.y);
                GridCellData exitCellData = CreatedGrid.GetValue(exitPointCoordinate.x, exitPointCoordinate.y);

                edge.SetEnterExitRoom(enterCellData, exitCellData);
            }

            return usedEdge;
        }

        foreach (Edge edge in allEdge)
        {
            if (additionalEdges >= RoomGeneratorSettings.AdditionSelectedEdges)
                break;

            if (usedEdge.Contains(edge))
                continue;

            var randomNumber = random.NextInt(0, 101);

            if (randomNumber > RoomGeneratorSettings.ChanceToSelectEdge)
                continue;

            usedEdge.Add(edge);
            additionalEdges++;
        }

        foreach (Edge edge in usedEdge)
        {
            var edgePoint1Vector2 = new Vector2((float)edge.Point1.X, (float)edge.Point1.Y);
            var edgePoint2Vector2 = new Vector2((float)edge.Point2.X, (float)edge.Point2.Y);
            

            (GridCellData entryPoint, GridCellData exitPoint) connectionPoints = FindConnectionPosition(edgePoint1Vector2, edgePoint2Vector2);

            // Sprawdzanie i scalanie punktów wejścia/wyjścia
            Vector2Int entryPointCoordinate = connectionPoints.entryPoint.Coordinate;
            Vector2Int exitPointCoordinate = connectionPoints.exitPoint.Coordinate;

            if (IsPointMerged(entryPointCoordinate, mergedPoints))
            {
                entryPointCoordinate = GetClosestMergedPoint(entryPointCoordinate, mergedPoints);
            }
            else
            {
                mergedPoints.Add(entryPointCoordinate);
            }

            if (IsPointMerged(exitPointCoordinate, mergedPoints))
            {
                exitPointCoordinate = GetClosestMergedPoint(exitPointCoordinate, mergedPoints);
            }
            else
            {
                mergedPoints.Add(exitPointCoordinate);
            }
            
            GridCellData enterCellData = CreatedGrid.GetValue(entryPointCoordinate.x, entryPointCoordinate.y);
            GridCellData exitCellData = CreatedGrid.GetValue(exitPointCoordinate.x, exitPointCoordinate.y);

            edge.SetEnterExitRoom(enterCellData, exitCellData);
        }

        return usedEdge;
    }


    public (GridCellData entryPoint, GridCellData exitPoint) FindConnectionPosition(NewRoom room1, NewRoom room2)
    {
        // Oblicz centroidy pokojów jako punkty odniesienia
        Vector2 room1Center = new Vector2(room1.Centroid.x, room1.Centroid.z);
        Vector2 room2Center = new Vector2(room2.Centroid.x, room2.Centroid.z);

        // Oblicz różnicę współrzędnych
        float dx = room2Center.x - room1Center.x;
        float dy = room2Center.y - room1Center.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        // Znormalizowany kierunek
        float nx = dx / distance;
        float ny = dy / distance;

        // Wyznacz punkty docelowe na podstawie kierunku
        Vector2 entryPointCandidate = room1Center + new Vector2(nx, ny) * 0.5f; // Punkt w kierunku wyjścia
        Vector2 exitPointCandidate = room2Center - new Vector2(nx, ny) * 0.5f; // Punkt w kierunku wejścia

        // Znajdź najbliższe komórki na krawędziach pokojów
        GridCellData entryPoint = FindClosestEdgeCellToDirection(entryPointCandidate, room1);
        GridCellData exitPoint = FindClosestEdgeCellToDirection(exitPointCandidate, room2);

        return (entryPoint, exitPoint);
    }

    public (GridCellData entryPoint, GridCellData exitPoint) FindConnectionPosition(Vector2 point1, Vector2 point2)
    {
        // Oblicz centroidy pokojów jako punkty odniesienia

        // Oblicz różnicę współrzędnych
        float dx = point2.x - point1.x;
        float dy = point2.y - point1.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        // Znormalizowany kierunek
        float nx = dx / distance;
        float ny = dy / distance;

        // Wyznacz punkty docelowe na podstawie kierunku
        Vector2 entryPointCandidate = point1 + new Vector2(nx, ny) * 0.5f; // Punkt w kierunku wyjścia
        Vector2 exitPointCandidate = point2 - new Vector2(nx, ny) * 0.5f; // Punkt w kierunku wejścia

        return (CreatedGrid.GetValue((int)entryPointCandidate.x, (int)entryPointCandidate.y),
            CreatedGrid.GetValue((int)exitPointCandidate.x, (int)exitPointCandidate.y));
    }

    private GridCellData FindClosestEdgeCellToDirection(Vector2 candidatePoint, NewRoom room)
    {
        GridCellData closestCell = null;
        float minDistance = float.MaxValue;

        foreach (var cell in room.CellInRoom)
        {
            // Sprawdź, czy komórka jest na krawędzi pokoju
            if (IsEdgeCell(cell, room))
            {
                float cellX = cell.Coordinate.x;
                float cellY = cell.Coordinate.y;

                // Oblicz odległość od punktu kandydata do komórki
                float distance = Vector2.Distance(candidatePoint, new Vector2(cellX, cellY));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCell = cell;
                }
            }
        }

        return closestCell;
    }

    private bool IsEdgeCell(GridCellData cell, NewRoom room)
    {
        // Krawędź pokoju - znajdź minimalne i maksymalne wartości x i y
        int minX = room.CellInRoom.Min(c => c.Coordinate.x);
        int maxX = room.CellInRoom.Max(c => c.Coordinate.x);
        int minY = room.CellInRoom.Min(c => c.Coordinate.y);
        int maxY = room.CellInRoom.Max(c => c.Coordinate.y);

        // Komórka jest na krawędzi, jeśli jej współrzędne są równe minimalnym lub maksymalnym
        return cell.Coordinate.x == minX || cell.Coordinate.x == maxX ||
               cell.Coordinate.y == minY || cell.Coordinate.y == maxY;
    }

    private bool IsPointMerged(Vector2Int point, HashSet<Vector2Int> mergedPoints)
    {
        foreach (var p in mergedPoints)
        {
            if (Vector2Int.Distance(point, p) <= 1) // Tolerancja <= 1 jednostki (dla grida)
            {
                return true;
            }
        }

        return false;
    }

    private Vector2Int GetClosestMergedPoint(Vector2Int point, HashSet<Vector2Int> mergedPoints)
    {
        Vector2Int closestPoint = point;
        float minDistance = float.MaxValue;

        foreach (var p in mergedPoints)
        {
            float distance = Vector2Int.Distance(point, p); // Obliczenie odległości
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = p;
            }
        }

        return closestPoint;
    }

    private void OnDrawGizmos()
    {
        Color color = Color.white;
        foreach (var room in RoomGeneratorSettings.AllCreatedRooms)
        {
            Gizmos.DrawSphere(room.Centroid + new Vector3(0, 3, 0), 0.2f);
            Gizmos.DrawLine(room.Centroid, room.Centroid + new Vector3(0, 3, 0));
        }


        if (AllEdges == null) return;

        Gizmos.color = Color.green;

        foreach (var edge in AllEdges)
        {
            if (SelectedEdges.Contains(edge))
                continue;

            Vector3 start = new Vector3((float)edge.Point1.X, 0.1f, (float)edge.Point1.Y);
            Vector3 end = new Vector3((float)edge.Point2.X, 0.1f, (float)edge.Point2.Y);

            Gizmos.DrawLine(start, end);
        }

        Gizmos.color = Color.blue;

        foreach (var edge in SelectedEdges)
        {
            Vector3 start = new Vector3((float)edge.Point1.X, 0.1f, (float)edge.Point1.Y);
            Vector3 end = new Vector3((float)edge.Point2.X, 0.1f, (float)edge.Point2.Y);

            Gizmos.DrawLine(start, end);
        }
    }
}
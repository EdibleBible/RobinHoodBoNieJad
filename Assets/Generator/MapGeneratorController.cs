using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using TMPro;
using System.Xml.Linq;
using System;
using Unity.Mathematics;

public class MapGeneratorController : MonoBehaviour
{
    public GridOptions MainGridData;
    public RoomGanerateSetting RoomGanerateSetting;

    public DelaunayTriangulator Triangulator;
    public List<Point> AllPoints;
    public List<Edge> AllEdges;
    public List<Edge> SelectedEdges;

    //GridCellDataGrid (mainInfoGrid)
    public CustomGrid.Grid<GridCellData> MainInfoGrid;
    public Pathfinding currPathfinding;
    public CustomGrid.Grid<PathNode> pathfindingGrid;

    [Header("Textures")]
    [SerializeField] private Vector2 segmentSize;
    [SerializeField] private List<Mesh> wallMeshes;
    [SerializeField] private List<Mesh> passesMeshes;
    [SerializeField] private List<Mesh> hallwayMeshes;
    [SerializeField] private Material wallMaterial;

    Dictionary<Mesh, List<Matrix4x4>> WallsMatrix = new Dictionary<Mesh, List<Matrix4x4>>();

    [Header("Seed")]
    public uint wallSeed;
    private Unity.Mathematics.Random random;

    [Header("Debug")]
    private Dictionary<GridCellData, GameObject> allObject = new Dictionary<GridCellData, GameObject>();
    [SerializeField] private TextMeshProUGUI ActionTextMesh;
    [SerializeField] private TextMeshProUGUI InstructionTextMesh;
    [SerializeField] private TextMeshProUGUI SeedTextMesh;

    public uint SetSeed(uint setValue)
    {
        wallSeed = setValue;
        random = new Unity.Mathematics.Random(wallSeed);
        return wallSeed;
    }
    public uint RandimizeSeed()
    {
        var randimInt = new System.Random().Next(1000, 9999);
        wallSeed = (uint)randimInt;
        random = new Unity.Mathematics.Random(wallSeed);
        return wallSeed;
    }

    //Playmode Method
    private void Start()
    {
        StartGeneration();
    }

    private bool isDrawing = false;

    void Update()
    {
        DrawWalls();
    }

    public void StartGeneration()
    {
        ClearGeneratedGrid();

        RandimizeSeed();
        UpdateSeedText(wallSeed.ToString());

        UpdateActionText("Clearing grid...");
        StartCoroutine(Generate());
    }

    private void ClearGeneratedGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        MainInfoGrid = null;
    }

    private IEnumerator Generate()
    {
        // Generate grid
        UpdateActionText("Generating grid and rooms...");
        UpdateInstructionText("Press SPACE to continue.");
        GenerateGrid(wallSeed);


        // Create rooms on grid
        RoomGanerateSetting.CreateRoomsOnGrid(MainInfoGrid, wallSeed);
        DebugGridMesh();
        yield return WaitForSpaceBar();

        // Generate triangulation
        UpdateActionText("Generating triangulation...");
        UpdateInstructionText("Press SPACE to continue.");
        GenerateTriangulation();
        DebugGridMesh();
        yield return WaitForSpaceBar();

        // Set selected edges
        UpdateActionText("Setting selected edges...");
        UpdateInstructionText("Press SPACE to continue.");
        SelectedEdges = GetUsedEdges(AllEdges, AllPoints);
        DebugGridMesh();
        yield return WaitForSpaceBar();

        // Run pathfinding for rooms
        UpdateActionText("Running pathfinding for rooms...");
        UpdateInstructionText("Press SPACE to continue.");
        yield return StartCoroutine(RoomPathFindWithDebugging());
        yield return WaitForSpaceBar();

        // Define spawn points
        UpdateActionText("Defining spawn points...");
        UpdateInstructionText("Press SPACE to continue.");
        DefiniedSpawn();
        DebugGridMesh();
        GenerateTexture();
        yield return WaitForSpaceBar();

        // Debug the grid mesh
        UpdateActionText("Rese room");
        UpdateInstructionText("Press SPACE to finish.");

        yield return WaitForSpaceBar();
        StartGeneration();
    }


    private void UpdateSeedText(string seedText)
    {
        // Update the action text (assign this to your action TextMesh object)
        SeedTextMesh.text = "SEED: " + seedText;
    }

    private void UpdateActionText(string actionText)
    {
        // Update the action text (assign this to your action TextMesh object)
        ActionTextMesh.text = actionText;
    }

    private void UpdateInstructionText(string instructionText)
    {
        // Update the instruction text (assign this to your instruction TextMesh object)
        InstructionTextMesh.text = instructionText;
    }
    private IEnumerator WaitForSpaceBar()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
    }

    //Grid Debuging
    public void DebugGridMesh()
    {
        if (MainInfoGrid == null)
            return;

        Vector2Int gridSize = MainInfoGrid.GeneratedGridSize();
        var coppy = allObject;

        foreach (var cellData in coppy)
        {
            Destroy(allObject.Where(x => x.Key == cellData.Key).FirstOrDefault().Value);
        }

        allObject.Clear();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var cell = MainInfoGrid.GetValue(x, y);
                if (cell == null) continue;

                // Tworzenie komórki
                var createdCell = new GameObject($"cell nr x:{cell.Coordinate.x} y:{cell.Coordinate.y}");
                createdCell.transform.parent = transform;

                // Przypisanie pozycji
                createdCell.transform.position = cell.Position;

                // Dodanie komponentów wizualnych
                MeshFilter meshFilter = createdCell.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

                MeshRenderer meshRenderer = createdCell.AddComponent<MeshRenderer>();
                switch (cell.GridCellType)
                {
                    case E_GridCellType.Empty:
                        DestroyImmediate(createdCell.gameObject);
                        if (createdCell != null)
                        {
                            Destroy(createdCell.gameObject);
                            createdCell = null;

                        }
                        break;

                    case E_GridCellType.Room:
                        meshRenderer.material = MainGridData.RoomCellMaterial;
                        break;

                    case E_GridCellType.Hallway:
                        meshRenderer.material = MainGridData.HallwayMaterial;
                        break;

                    case E_GridCellType.Pass:
                        meshRenderer.material = MainGridData.PassCellMaterial;
                        break;

                    case E_GridCellType.SpawnRoom:
                        meshRenderer.material = MainGridData.SpawnRoomMaterial;
                        break;

                    case E_GridCellType.SpawnPass:
                        meshRenderer.material = MainGridData.SpawnPassMaterial;
                        break;

                    default:
                        Debug.LogWarning($"Unknown GridCellType: {cell.GridCellType}");
                        meshRenderer.material = null; // Możesz ustawić materiał domyślny
                        break;
                }

                if (createdCell == null)
                    continue;
                // Skalowanie
                createdCell.transform.localScale = Vector3.one * MainGridData.cellScale;

                // Dodanie do słownika dla debugowania
                allObject.Add(cell, createdCell);
            }
        }
    }
    public void DebugSingleCellMesh(GridCellData cell)
    {
        if (cell == null)
            return;

        if (allObject.ContainsKey(cell) && allObject[cell] != null)
        {
            Destroy(allObject[cell].gameObject);
        }


        // Tworzenie komórki
        var createdCell = new GameObject($"cell nr x:{cell.Coordinate.x} y:{cell.Coordinate.y}");
        createdCell.transform.parent = transform;

        // Przypisanie pozycji
        createdCell.transform.position = cell.Position;

        // Dodanie komponentów wizualnych
        MeshFilter meshFilter = createdCell.AddComponent<MeshFilter>();
        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

        MeshRenderer meshRenderer = createdCell.AddComponent<MeshRenderer>();
        switch (cell.GridCellType)
        {
            case E_GridCellType.Empty:
                DestroyImmediate(createdCell.gameObject);
                if (createdCell != null)
                {
                    Destroy(createdCell.gameObject);
                    createdCell = null;

                }
                break;

            case E_GridCellType.Room:
                meshRenderer.material = MainGridData.RoomCellMaterial;
                break;

            case E_GridCellType.Hallway:
                meshRenderer.material = MainGridData.HallwayMaterial;
                break;

            case E_GridCellType.Pass:
                meshRenderer.material = MainGridData.PassCellMaterial;
                break;

            case E_GridCellType.SpawnRoom:
                meshRenderer.material = MainGridData.SpawnRoomMaterial;
                break;

            case E_GridCellType.SpawnPass:
                meshRenderer.material = MainGridData.SpawnPassMaterial;
                break;

            default:
                Debug.LogWarning($"Unknown GridCellType: {cell.GridCellType}");
                meshRenderer.material = null; // Możesz ustawić materiał domyślny
                break;
        }

        if (createdCell == null)
            return;

        // Skalowanie
        createdCell.transform.localScale = Vector3.one * MainGridData.cellScale;

        // Dodanie do słownika dla debugowania
        if (allObject.ContainsKey(cell))
        {
            allObject[cell] = createdCell;
        }
        else
        {
            allObject.Add(cell, createdCell);
        }
    }
    public IEnumerator RoomPathFindWithDebugging()
    {
        if (MainInfoGrid == null)
            yield break;

        List<GridCellData> roomCell = new List<GridCellData>();

        // Zbieramy komórki, które nie są przejściowe.
        foreach (var room in RoomGanerateSetting.CreatedRoom)
        {
            foreach (var cell in room.CellInRoom)
            {
                /*                if (cell.GridCellType != E_GridCellType.Pass)
                */
                roomCell.Add(cell);
            }
        }

        var array = MainInfoGrid.GetGridArray();
        List<GridCellData> list = new List<GridCellData>();
        foreach (var item in array)
        {
            list.Add(item);
        }

        // Inicjalizacja pathfinding
        currPathfinding = new Pathfinding(
            MainInfoGrid.GetWidth(),
            MainInfoGrid.GetHeight(),
            MainGridData.cellScale,
            transform.position,
            roomCell,
            list
        );

        pathfindingGrid = currPathfinding.GetGrid();

        foreach (Edge edge in SelectedEdges)
        {
            // Walidacja krawędzi i jej danych
            if (edge == null || edge.EntryGridCell == null || edge.ExitGridCell == null ||
                edge.EntryGridCell.Coordinate == null || edge.ExitGridCell.Coordinate == null)
            {
                yield break;
            }

            // Znajdowanie ścieżki
            var startPointCord = GetStartCoordinate(edge.EntryGridCell);
            var endPointCord = GetStartCoordinate(edge.ExitGridCell);

            edge.SetStartEndPathfind(MainInfoGrid.GetValue(startPointCord.x, startPointCord.y), MainInfoGrid.GetValue(endPointCord.x, endPointCord.y));

            // Znajdowanie ścieżki
            List<PathNode> pathNodeCell = currPathfinding.FindPath(
                startPointCord.x,
                startPointCord.y,
                endPointCord.x,
                endPointCord.y
            );

            if (pathNodeCell == null)
                continue;

            var obj1Mesh = allObject[edge.EntryGridCell].GetComponent<MeshRenderer>();
            var obj2Mesh = allObject[edge.ExitGridCell].GetComponent<MeshRenderer>();

            obj1Mesh.material = MainGridData.SecretHallwayMaterial;
            obj2Mesh.material = MainGridData.SecretPassMaterial;

            // Zaktualizowanie komórek jako 'Hallway'
            foreach (var node in pathNodeCell)
            {
                if ((node.X == edge.EntryGridCell.Coordinate.x && node.Y == edge.EntryGridCell.Coordinate.y) ||
                    (node.X == edge.ExitGridCell.Coordinate.x && node.Y == edge.ExitGridCell.Coordinate.y))
                    continue;

                GridCellData toAdd = MainInfoGrid.GetValue(node.X, node.Y);
                Debug.LogWarning($"You move to grid cords X: {toAdd.Coordinate.x} Y: {toAdd.Coordinate.y} Node is: {currPathfinding.GetGrid().GetValue(toAdd.Coordinate.x, toAdd.Coordinate.y).IsWalkable} And Type: {toAdd.GridCellType}");
                toAdd.GridCellType = E_GridCellType.Hallway;

                // Debugowanie pojedynczej kratki
                DebugSingleCellMesh(toAdd);

                // Pauza między kratkami
                yield return new WaitForSeconds(0.1f);
            }

            obj1Mesh.material = MainGridData.PassCellMaterial;
            obj2Mesh.material = MainGridData.PassCellMaterial;

            // Jeśli ścieżka nie może być dotknięta, sprawdzamy sąsiadów
            if (!currPathfinding.CAN_PATH_TOUCHED)
            {
                foreach (var currentNode in pathNodeCell)
                {
                    var neighbors = pathfindingGrid.GetNeighbourList(currentNode, false);

                    foreach (var neighbor in neighbors)
                    {
                        // Pomijamy sąsiadów na skos
                        if (Mathf.Abs(neighbor.X - currentNode.X) == 1 && Mathf.Abs(neighbor.Y - currentNode.Y) == 1)
                            continue;

                        var neighborCell = MainInfoGrid.GetValue(neighbor.X, neighbor.Y);
                        var neighborNeighbors = pathfindingGrid.GetNeighbourList(neighbor, true);

                        bool isNextToPassableCell = false;
                        foreach (var neighborOfNeighbor in neighborNeighbors)
                        {
                            var neighborOfNeighborCell = MainInfoGrid.GetValue(neighborOfNeighbor.X, neighborOfNeighbor.Y);
                            if (neighborOfNeighborCell.GridCellType == E_GridCellType.Pass)
                            {
                                isNextToPassableCell = true;
                                break;
                            }
                        }

                        // Jeśli sąsiad sąsiada jest przejściowy, pomijamy go
                        if (isNextToPassableCell)
                            continue;

                        // Jeśli sąsiad jest pustą komórką, oznaczamy jako nieprzechodni
                        if (neighborCell.GridCellType == E_GridCellType.Empty)
                        {
                            neighbor.IsWalkable = false;
                        }
                    }
                }
            }
        }
    }

    public void GenerateGrid(uint seed)
    {
        var randomSize = MainGridData.RandomizeGridSize(seed);
        GenerateGrid(randomSize.x, randomSize.y);
    }
    public void GenerateGrid(int gridX, int gridY)
    {
        // Tworzenie głównej siatki
        MainInfoGrid = new CustomGrid.Grid<GridCellData>(gridX, gridY, MainGridData.cellScale, transform.position, (CustomGrid.Grid<GridCellData> g, int x, int y) =>
        {
            GridCellData cellData = new GridCellData();
            cellData.SetCoordinate(x, y);

            // Obliczanie pozycji (start od 0, 0)
            Vector3 position = new Vector3(x * MainGridData.cellScale, 0, y * MainGridData.cellScale);
            cellData.SetPosition(position);
            return cellData;
        });

        foreach (var gridElement in MainInfoGrid.GetGridArray())
        {
            gridElement.SetGridParent(MainInfoGrid);
        }
    }

    public (GridCellData entryPoint, GridCellData exitPoint) FindConnectionPosition(Room room1, Room room2)
    {
        // Oblicz centroidy pokojów jako punkty odniesienia
        Vector2 room1Center = new Vector2(room1.cetroid.x, room1.cetroid.z);
        Vector2 room2Center = new Vector2(room2.cetroid.x, room2.cetroid.z);

        // Oblicz różnicę współrzędnych
        float dx = room2Center.x - room1Center.x;
        float dy = room2Center.y - room1Center.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        // Znormalizowany kierunek
        float nx = dx / distance;
        float ny = dy / distance;

        // Wyznacz punkty docelowe na podstawie kierunku
        Vector2 entryPointCandidate = room1Center + new Vector2(nx, ny) * 0.5f; // Punkt w kierunku wyjścia
        Vector2 exitPointCandidate = room2Center - new Vector2(nx, ny) * 0.5f;  // Punkt w kierunku wejścia

        // Znajdź najbliższe komórki na krawędziach pokojów
        GridCellData entryPoint = FindClosestEdgeCellToDirection(entryPointCandidate, room1);
        GridCellData exitPoint = FindClosestEdgeCellToDirection(exitPointCandidate, room2);

        return (entryPoint, exitPoint);
    }
    private GridCellData FindClosestEdgeCellToDirection(Vector2 candidatePoint, Room room)
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
    private bool IsEdgeCell(GridCellData cell, Room room)
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
    public List<Triangle> GenerateTriangulation()
    {
        Triangulator = new DelaunayTriangulator();
        List<Point> listOfPoints = new List<Point>();

        // Tworzenie punktów
        foreach (Room roomCentre in RoomGanerateSetting.CreatedRoom)
        {
            Point newPoint = new Point(roomCentre.cetroid.x, roomCentre.cetroid.z);
            newPoint.SetPointRoom(roomCentre);
            listOfPoints.Add(newPoint);
        }

        // Generowanie triangulacji
        List<Triangle> allTriangle = Triangulator.BowyerWatson(listOfPoints).ToList();


        AllEdges = GetEdgesFromTriangles(allTriangle);
        AllPoints = GetPointsFromTriangles(allTriangle);
        return allTriangle;
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
    public List<Edge> GetUsedEdges(List<Edge> allEdge, List<Point> allPoints)
    {
        List<Edge> usedEdge = PrimAlgorithm.FindMST(allEdge, allPoints);
        int additionalEdges = 0;
        HashSet<Vector2Int> mergedPoints = new HashSet<Vector2Int>();
        if (!RoomGanerateSetting.UseAdditionalEdges)
        {

            foreach (Edge edge in usedEdge)
            {
                var edgePoint1Vector2 = new Vector2((float)edge.Point1.X, (float)edge.Point1.Y);
                var edgePoint2Vector2 = new Vector2((float)edge.Point2.X, (float)edge.Point2.Y);

                var room1 = RoomGanerateSetting.CreatedRoom
                    .FirstOrDefault(x => x.cetroid.x == edgePoint1Vector2.x && x.cetroid.z == edgePoint1Vector2.y);
                var room2 = RoomGanerateSetting.CreatedRoom
                    .FirstOrDefault(x => x.cetroid.x == edgePoint2Vector2.x && x.cetroid.z == edgePoint2Vector2.y);

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

                GridCellData enterCellData = MainInfoGrid.GetValue(entryPointCoordinate.x, entryPointCoordinate.y);
                GridCellData exitCellData = MainInfoGrid.GetValue(exitPointCoordinate.x, exitPointCoordinate.y);

                edge.SetEnterExitRoom(enterCellData, exitCellData);
            }

            return usedEdge;
        }

        foreach (Edge edge in allEdge)
        {
            if (additionalEdges >= RoomGanerateSetting.AdditionSelectedEdges)
                break;

            if (usedEdge.Contains(edge))
                continue;

            var randomNumber = random.NextInt(0, 101);

            if (randomNumber > RoomGanerateSetting.ChanceToSelectEdge)
                continue;

            usedEdge.Add(edge);
            additionalEdges++;
        }
        foreach (Edge edge in usedEdge)
        {
            var edgePoint1Vector2 = new Vector2((float)edge.Point1.X, (float)edge.Point1.Y);
            var edgePoint2Vector2 = new Vector2((float)edge.Point2.X, (float)edge.Point2.Y);

            var room1 = RoomGanerateSetting.CreatedRoom
                .FirstOrDefault(x => x.cetroid.x == edgePoint1Vector2.x && x.cetroid.z == edgePoint1Vector2.y);
            var room2 = RoomGanerateSetting.CreatedRoom
                .FirstOrDefault(x => x.cetroid.x == edgePoint2Vector2.x && x.cetroid.z == edgePoint2Vector2.y);

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

            GridCellData enterCellData = MainInfoGrid.GetValue(entryPointCoordinate.x, entryPointCoordinate.y);
            GridCellData exitCellData = MainInfoGrid.GetValue(exitPointCoordinate.x, exitPointCoordinate.y);

            edge.SetEnterExitRoom(enterCellData, exitCellData);
        }

        return usedEdge;
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
    public void RoomPathFind()
    {
        if (MainInfoGrid == null)
            return;

        List<GridCellData> roomCell = new List<GridCellData>();

        // Zbieramy komórki, które nie są przejściowe.
        foreach (var room in RoomGanerateSetting.CreatedRoom)
        {
            foreach (var cell in room.CellInRoom)
            {
                /*                if (cell.GridCellType != E_GridCellType.Pass)
                */
                roomCell.Add(cell);
            }
        }

        var array = MainInfoGrid.GetGridArray();
        List<GridCellData> list = new List<GridCellData>();
        foreach (var item in array)
        {
            list.Add(item);
        }

        // Inicjalizacja pathfinding
        currPathfinding = new Pathfinding(
            MainInfoGrid.GetWidth(),
            MainInfoGrid.GetHeight(),
            MainGridData.cellScale,
            transform.position,
            roomCell,
            list
        );


        pathfindingGrid = currPathfinding.GetGrid();

        foreach (Edge edge in SelectedEdges)
        {
            // Walidacja krawędzi i jej danych
            if (edge == null || edge.EntryGridCell == null || edge.ExitGridCell == null ||
                edge.EntryGridCell.Coordinate == null || edge.ExitGridCell.Coordinate == null)
            {
                return;
            }

            var startPointCord = GetStartCoordinate(edge.EntryGridCell);
            var endPointCord = GetStartCoordinate(edge.ExitGridCell);

            edge.SetStartEndPathfind(MainInfoGrid.GetValue(startPointCord.x, startPointCord.y), MainInfoGrid.GetValue(endPointCord.x, endPointCord.y));

            // Znajdowanie ścieżki
            List<PathNode> pathNodeCell = currPathfinding.FindPath(
                startPointCord.x,
                startPointCord.y,
                endPointCord.x,
                endPointCord.y
            );

            if (pathNodeCell == null)
                continue;

            // Zaktualizowanie komórek jako 'Hallway'
            foreach (var node in pathNodeCell)
            {
                if ((node.X == edge.EntryGridCell.Coordinate.x && node.Y == edge.EntryGridCell.Coordinate.y) ||
                    (node.X == edge.ExitGridCell.Coordinate.x && node.Y == edge.ExitGridCell.Coordinate.y))
                    continue;

                GridCellData toAdd = MainInfoGrid.GetValue(node.X, node.Y);
                toAdd.GridCellType = E_GridCellType.Hallway;
            }

            // Jeśli ścieżka nie może być dotknięta, sprawdzamy sąsiadów
            if (!currPathfinding.CAN_PATH_TOUCHED)
            {
                foreach (var currentNode in pathNodeCell)
                {
                    var neighbors = pathfindingGrid.GetNeighbourList(currentNode, false);

                    foreach (var neighbor in neighbors)
                    {
                        // Pomijamy sąsiadów na skos
                        if (Mathf.Abs(neighbor.X - currentNode.X) == 1 && Mathf.Abs(neighbor.Y - currentNode.Y) == 1)
                            continue;

                        var neighborCell = MainInfoGrid.GetValue(neighbor.X, neighbor.Y);
                        var neighborNeighbors = pathfindingGrid.GetNeighbourList(neighbor, true);

                        bool isNextToPassableCell = false;
                        foreach (var neighborOfNeighbor in neighborNeighbors)
                        {
                            var neighborOfNeighborCell = MainInfoGrid.GetValue(neighborOfNeighbor.X, neighborOfNeighbor.Y);
                            if (neighborOfNeighborCell.GridCellType == E_GridCellType.Pass)
                            {
                                isNextToPassableCell = true;
                                break;
                            }
                        }

                        // Jeśli sąsiad sąsiada jest przejściowy, pomijamy go
                        if (isNextToPassableCell)
                            continue;

                        // Jeśli sąsiad jest pustą komórką, oznaczamy jako nieprzechodni
                        if (neighborCell.GridCellType == E_GridCellType.Empty)
                        {
                            neighbor.IsWalkable = false;
                        }
                    }
                }
            }
        }
        return;
    }
    private Vector2Int GetStartCoordinate(GridCellData selectedGridCellData)
    {
        GridCellData baseStartPoint = selectedGridCellData;
        List<PathNode> allNeighbourStart = pathfindingGrid.GetNeighbourList(pathfindingGrid.GetValue(baseStartPoint.Coordinate.x, baseStartPoint.Coordinate.y), false);
        List<GridCellData> selectedNeighbourStart = new List<GridCellData>();
        foreach (var element in allNeighbourStart)
        {
            GridCellData cell = MainInfoGrid.GetValue(element.X, element.Y);
            if (cell.GridCellType != E_GridCellType.Room && cell.GridCellType != E_GridCellType.Pass)
                selectedNeighbourStart.Add(cell);
        }
        if (selectedNeighbourStart.Count > 0)
        {

            int randomIndex = random.NextInt(0, selectedNeighbourStart.Count);
            GridCellData selectedCell = selectedNeighbourStart[randomIndex];
            return selectedCell.Coordinate;
        }
        return default;
    }
    public void DefiniedSpawn()
    {
        int biggestRoomGridCount = 0;
        foreach (var room in RoomGanerateSetting.CreatedRoom)
        {
            if (room.CellInRoom.Count() > biggestRoomGridCount)
                biggestRoomGridCount = room.CellInRoom.Count();
        }

        List<Room> biggestRooms = new List<Room>();

        foreach (var room in RoomGanerateSetting.CreatedRoom)
        {
            if (room.CellInRoom.Count() == biggestRoomGridCount)
                biggestRooms.Add(room);
        }

        int passAmount = 0;

        foreach (var room in biggestRooms)
        {
            int passCount = 0;
            foreach (var grid in room.CellInRoom)
            {
                if (grid.GridCellType == E_GridCellType.Pass)
                    passCount++;
            }

            if (passCount > passAmount)
                passAmount = passCount;
        }

        List<Room> biggestRoomsWithMostPass = new List<Room>();

        foreach (var room in biggestRooms)
        {
            int passCount = 0;
            foreach (var grid in room.CellInRoom)
            {
                if (grid.GridCellType == E_GridCellType.Pass)
                    passCount++;
            }

            if (passCount == passAmount)
                biggestRoomsWithMostPass.Add(room);
        }

        Room randomRoom = biggestRoomsWithMostPass[UnityEngine.Random.RandomRange(0, biggestRoomsWithMostPass.Count - 1)];
        randomRoom.RoomType = E_RoomType.SpawnRoom;

        foreach (var cell in randomRoom.CellInRoom)
        {
            if (cell.GridCellType == E_GridCellType.Pass)
            {
                cell.GridCellType = E_GridCellType.SpawnPass;

            }
            else if (cell.GridCellType == E_GridCellType.Room)
            {
                cell.GridCellType = E_GridCellType.SpawnRoom;
            }
        }

    }


    public void GenerateTexture()
    {
        WallsMatrix = new Dictionary<Mesh, List<Matrix4x4>>();

        foreach (Room room in RoomGanerateSetting.CreatedRoom)
        {
            //x
            int wallCount = Mathf.Max(0, (int)(room.XAxisSize / segmentSize.x));
            float scale = room.XAxisSize / wallCount / segmentSize.x;

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = room.cetroid + new Vector3((-room.XAxisSize / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0, room.YAxisSize / 2f);
                Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);

                int randomNumber = random.NextInt(0, wallMeshes.Count);

                if (WallsMatrix.ContainsKey(wallMeshes[randomNumber]))
                {
                    WallsMatrix[wallMeshes[randomNumber]].Add(newMatrix);
                }
                else
                {
                    WallsMatrix.Add(wallMeshes[randomNumber], new List<Matrix4x4>() { newMatrix });
                }
            }

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = room.cetroid + new Vector3((-room.XAxisSize / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0, -room.YAxisSize / 2f);
                Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                int randomNumber = random.NextInt(0, wallMeshes.Count);

                if (WallsMatrix.ContainsKey(wallMeshes[randomNumber]))
                {
                    WallsMatrix[wallMeshes[randomNumber]].Add(newMatrix);
                }
                else
                {
                    WallsMatrix.Add(wallMeshes[randomNumber], new List<Matrix4x4>() { newMatrix });
                }
            }

            //y

            wallCount = Mathf.Max(0, (int)(room.YAxisSize / segmentSize.y));
            scale = room.YAxisSize / wallCount / segmentSize.y;
            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = room.cetroid + new Vector3(room.XAxisSize / 2f, 0, (-room.YAxisSize / 2f) + (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
                Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                int randomNumber = random.NextInt(0, wallMeshes.Count);

                if (WallsMatrix.ContainsKey(wallMeshes[randomNumber]))
                {
                    WallsMatrix[wallMeshes[randomNumber]].Add(newMatrix);
                }
                else
                {
                    WallsMatrix.Add(wallMeshes[randomNumber], new List<Matrix4x4>() { newMatrix });
                }
            }

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = room.cetroid + new Vector3(-room.XAxisSize / 2f, 0, (-room.YAxisSize / 2f) + (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
                Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                int randomNumber = random.NextInt(0, wallMeshes.Count);

                if (WallsMatrix.ContainsKey(wallMeshes[randomNumber]))
                {
                    WallsMatrix[wallMeshes[randomNumber]].Add(newMatrix);
                }
                else
                {
                    WallsMatrix.Add(wallMeshes[randomNumber], new List<Matrix4x4>() { newMatrix });
                }
            }
        }
    }

    public void DrawWalls()
    {
        foreach (var element in WallsMatrix)
        {
            Graphics.DrawMeshInstanced(element.Key, 0, wallMaterial, element.Value.ToArray());
        }
    }
}




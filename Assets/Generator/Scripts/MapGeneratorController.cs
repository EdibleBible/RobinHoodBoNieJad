using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using TMPro;
using System.Xml.Linq;
using System;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine.AI;
using Unity.Collections.LowLevel.Unsafe;

public class MapGeneratorController : MonoBehaviour
{
    public Transform generatedRoomTransform;

    public GridOptions MainGridData;
    public RoomGanerateSetting RoomGanerateSetting;

    public DelaunayTriangulator Triangulator;
    public List<Point> AllPoints;
    public List<Edge> AllEdges;
    public List<Edge> SelectedEdges;
    public List<GridCellData> Hallwaycell = new List<GridCellData>();

    //GridCellDataGrid (mainInfoGrid)
    public CustomGrid.Grid<GridCellData> MainInfoGrid;
    public Pathfinding currPathfinding;
    public CustomGrid.Grid<PathNode> pathfindingGrid;

    [Header("NavMesh Settings")] [SerializeField]
    private NavMeshSurface navMeshSurface;

    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private LayerMask passableLayerMask;


    [Header("Textures")] [SerializeField] private Vector2 segmentSize;
    [SerializeField] private List<Mesh> wallMeshes;
    [SerializeField] private List<Mesh> passesMeshes;
    [SerializeField] private List<Mesh> hallwayMeshes;
    [SerializeField] private List<Mesh> floorMashes;
    [SerializeField] private Material meshesMaterial;
    [SerializeField] private Material passesMaterial;

    Dictionary<Mesh, List<Matrix4x4>> AllMatrix = new Dictionary<Mesh, List<Matrix4x4>>();

    [Header("Seed")] public uint wallSeed;
    private Unity.Mathematics.Random random;
    public SOLevel levelData;

    [Header("Debug")] [SerializeField] private bool _debug;
    public Dictionary<GridCellData, GameObject> allObject = new Dictionary<GridCellData, GameObject>();

    [SerializeField] private TextMeshProUGUI ActionTextMesh;
    [SerializeField] private TextMeshProUGUI InstructionTextMesh;
    [SerializeField] private TextMeshProUGUI SeedTextMesh;

    public uint SetSeed(uint setValue)
    {
        wallSeed = setValue;
        random = new Unity.Mathematics.Random(wallSeed);
        levelData.levelSeed = (int)setValue;
        return wallSeed;
    }

    public uint RandimizeSeed()
    {
        var randimInt = new System.Random().Next(1000, 9999);
        wallSeed = (uint)randimInt;
        random = new Unity.Mathematics.Random(wallSeed);
        levelData.levelSeed = (int)wallSeed;
        return wallSeed;
    }

    //Playmode Method
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        StartGeneration();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RoomGanerateSetting.DetectObjects();
        }
    }

    public void StartGeneration()
    {
        ClearGeneratedGrid();

        RandimizeSeed();
        UpdateSeedText(wallSeed.ToString());

        if (_debug)
        {
            UpdateActionText("Clearing grid...");
            StartCoroutine(DebugGeneration());
        }
        else
        {
            StartCoroutine(GenerateMap());
        }
    }

    private void ClearGeneratedGrid()
    {
        AllMatrix = new Dictionary<Mesh, List<Matrix4x4>>();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        MainInfoGrid = null;
    }

    private IEnumerator GenerateMap()
    {
        GenerateGrid(wallSeed);
        RoomGanerateSetting.CreateRoomsOnGrid(MainInfoGrid, wallSeed, generatedRoomTransform);
        DebugGridMesh();
        GenerateTriangulation();
        DebugGridMesh();
        SelectedEdges = GetUsedEdges(AllEdges, AllPoints);
        DebugGridMesh();
        Hallwaycell = new List<GridCellData>();
        GeneratePath();
        DefiniedSpawn();
        DebugGridMesh();
        foreach (var element in allObject)
        {
            element.Value.SetActive(false);
        }

        GenerateMeshes();
        BakeNavigation();
        RoomGanerateSetting.SpawnRoomInside();
        yield return new WaitForEndOfFrame();
        RoomGanerateSetting.DetectObjects();
        RoomGanerateSetting.SpawnDoor();
        RoomGanerateSetting.SpawnPlayer();
    }

    private IEnumerator DebugGeneration()
    {
        UpdateActionText("Generate wall");
        UpdateInstructionText("Click Space to generate wall");
        yield return WaitForSpaceBar();
        yield return new WaitForSeconds(0.5f);


        GenerateGrid(wallSeed);
        RoomGanerateSetting.CreateRoomsOnGrid(MainInfoGrid, wallSeed, generatedRoomTransform);
        DebugGridMesh();

        UpdateActionText("Generate triangulation");
        UpdateInstructionText("Click Space to generate triangulation");
        yield return WaitForSpaceBar();
        yield return new WaitForSeconds(0.5f);


        GenerateTriangulation();
        DebugGridMesh();

        UpdateActionText("Select Edges");
        UpdateInstructionText("Click Space to select edges");
        yield return WaitForSpaceBar();
        yield return new WaitForSeconds(0.5f);

        SelectedEdges = GetUsedEdges(AllEdges, AllPoints);
        DebugGridMesh();

        UpdateActionText("Create Hallways");
        UpdateInstructionText("Click Space to generate hallways");
        yield return WaitForSpaceBar();
        yield return new WaitForSeconds(0.5f);


        Hallwaycell = new List<GridCellData>();

        UpdateActionText("Select Spawn");
        UpdateInstructionText("Click Space to select player spawn");
        yield return StartCoroutine(RoomPathFindWithDebugging());
        yield return WaitForSpaceBar();
        yield return new WaitForSeconds(0.5f);


        DefiniedSpawn();
        DebugGridMesh();

        UpdateActionText("Generate Meshes");
        UpdateInstructionText("Click Space to generate meshes");
        yield return WaitForSpaceBar();
        yield return new WaitForSeconds(0.1f);


        foreach (var element in allObject)
        {
            element.Value.SetActive(false);
        }

        GenerateMeshes();

        UpdateActionText("Bake Navigation");
        UpdateInstructionText("Click Space to Bake Navigation");
        yield return WaitForSpaceBar();
        yield return new WaitForSeconds(0.1f);


        BakeNavigation();

        UpdateActionText("Spawn Object");
        UpdateInstructionText("Click Space to spawn object");

        yield return WaitForSpaceBar();
        yield return new WaitForSeconds(0.1f);

        RoomGanerateSetting.SpawnPlayer();
    }

    private void UpdateSeedText(string seedText)
    {
        if (SeedTextMesh != null)
            SeedTextMesh.text = "SEED: " + seedText;
    }

    private void UpdateActionText(string actionText)
    {
        if (ActionTextMesh != null)
            ActionTextMesh.text = actionText;
    }

    private void UpdateInstructionText(string instructionText)
    {
        if (InstructionTextMesh != null)
        {
            InstructionTextMesh.text = instructionText;
        }
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

    public void GeneratePath()
    {
        if (MainInfoGrid == null)
            return;

        List<GridCellData> roomCell = new List<GridCellData>();

        // Zbieramy komórki, które nie są przejściowe.
        foreach (var room in RoomGanerateSetting.CreatedRoom)
        {
            foreach (var cell in room.CellInRoom)
            {
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

            // Znajdowanie ścieżki
            var startPointCord = GetStartCoordinate(edge.EntryGridCell);
            var endPointCord = GetStartCoordinate(edge.ExitGridCell);

            edge.SetStartPathFind(MainInfoGrid.GetValue(startPointCord.x, startPointCord.y));
            edge.SetEndPathFind(MainInfoGrid.GetValue(endPointCord.x, endPointCord.y));

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
                toAdd.GridCellType = E_GridCellType.Hallway;
                Hallwaycell.Add(toAdd);
                DebugSingleCellMesh(toAdd);
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
                            var neighborOfNeighborCell =
                                MainInfoGrid.GetValue(neighborOfNeighbor.X, neighborOfNeighbor.Y);
                            if (neighborOfNeighborCell.GridCellType == E_GridCellType.Pass)
                            {
                                isNextToPassableCell = true;
                                break;
                            }
                        }

                        if (isNextToPassableCell)
                            continue;

                        if (neighborCell.GridCellType == E_GridCellType.Empty)
                        {
                            neighbor.IsWalkable = false;
                        }
                    }
                }
            }
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

            edge.SetStartPathFind(MainInfoGrid.GetValue(startPointCord.x, startPointCord.y));
            edge.SetEndPathFind(MainInfoGrid.GetValue(endPointCord.x, endPointCord.y));

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
                Debug.LogWarning(
                    $"You move to grid cords X: {toAdd.Coordinate.x} Y: {toAdd.Coordinate.y} Node is: {currPathfinding.GetGrid().GetValue(toAdd.Coordinate.x, toAdd.Coordinate.y).IsWalkable} And Type: {toAdd.GridCellType}");
                toAdd.GridCellType = E_GridCellType.Hallway;
                Hallwaycell.Add(toAdd);
                // Debugowanie pojedynczej kratki
                DebugSingleCellMesh(toAdd);

                // Pauza między kratkami
                yield return new WaitForEndOfFrame();
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
                            var neighborOfNeighborCell =
                                MainInfoGrid.GetValue(neighborOfNeighbor.X, neighborOfNeighbor.Y);
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
        MainInfoGrid = new CustomGrid.Grid<GridCellData>(gridX, gridY, MainGridData.cellScale, transform.position,
            (CustomGrid.Grid<GridCellData> g, int x, int y) =>
            {
                GridCellData cellData = new GridCellData();
                cellData.SetCoordinate(x, y);
                cellData.SetCellSize(MainGridData.cellScale);

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
        Vector2 room1Center = new Vector2(room1.centroid.x, room1.centroid.z);
        Vector2 room2Center = new Vector2(room2.centroid.x, room2.centroid.z);

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
            Point newPoint = new Point(roomCentre.centroid.x, roomCentre.centroid.z);
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
                    .FirstOrDefault(x => x.centroid.x == edgePoint1Vector2.x && x.centroid.z == edgePoint1Vector2.y);
                var room2 = RoomGanerateSetting.CreatedRoom
                    .FirstOrDefault(x => x.centroid.x == edgePoint2Vector2.x && x.centroid.z == edgePoint2Vector2.y);

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
                .FirstOrDefault(x => x.centroid.x == edgePoint1Vector2.x && x.centroid.z == edgePoint1Vector2.y);
            var room2 = RoomGanerateSetting.CreatedRoom
                .FirstOrDefault(x => x.centroid.x == edgePoint2Vector2.x && x.centroid.z == edgePoint2Vector2.y);

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

            edge.SetStartPathFind(MainInfoGrid.GetValue(startPointCord.x, startPointCord.y));
            edge.SetEndPathFind(MainInfoGrid.GetValue(endPointCord.x, endPointCord.y));

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
                Hallwaycell.Add(toAdd);
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
                            var neighborOfNeighborCell =
                                MainInfoGrid.GetValue(neighborOfNeighbor.X, neighborOfNeighbor.Y);
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
        List<PathNode> allNeighbourStart =
            pathfindingGrid.GetNeighbourList(
                pathfindingGrid.GetValue(baseStartPoint.Coordinate.x, baseStartPoint.Coordinate.y), false);
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

        Room randomRoom =
            biggestRoomsWithMostPass[UnityEngine.Random.RandomRange(0, biggestRoomsWithMostPass.Count - 1)];
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
        AllMatrix = new Dictionary<Mesh, List<Matrix4x4>>();

        foreach (Room room in RoomGanerateSetting.CreatedRoom)
        {
            List<Matrix4x4> normalWallCell = new List<Matrix4x4>();
            List<Matrix4x4> passWallCell = new List<Matrix4x4>();
            List<Matrix4x4> floorCell = new List<Matrix4x4>();


            foreach (var roomCell in room.CellInRoom)
            {
                normalWallCell.AddRange(roomCell.RoomWallMatrix4s4(segmentSize));
                floorCell.AddRange(roomCell.FlorMatrix4x4(segmentSize));
            }

            foreach (var roomCell in room.CellInRoom)
            {
                passWallCell.AddRange(roomCell.RoomPassWallMatrix4x4(segmentSize));
            }

            foreach (var element in passWallCell)
            {
                int randomMeshIndex = random.NextInt(0, passesMeshes.Count);

                if (AllMatrix.ContainsKey(passesMeshes[randomMeshIndex]))
                {
                    AllMatrix[passesMeshes[randomMeshIndex]].Add(element);
                }
                else
                {
                    AllMatrix.Add(passesMeshes[randomMeshIndex], new List<Matrix4x4>() { element });
                }
            }

            foreach (var element in normalWallCell)
            {
                int randomMeshIndex = random.NextInt(0, wallMeshes.Count);

                if (AllMatrix.ContainsKey(wallMeshes[randomMeshIndex]))
                {
                    AllMatrix[wallMeshes[randomMeshIndex]].Add(element);
                }
                else
                {
                    AllMatrix.Add(wallMeshes[randomMeshIndex], new List<Matrix4x4>() { element });
                }
            }

            foreach (var element in floorCell)
            {
                int randomMeshIndex = random.NextInt(0, floorMashes.Count);

                if (AllMatrix.ContainsKey(floorMashes[randomMeshIndex]))
                {
                    AllMatrix[floorMashes[randomMeshIndex]].Add(element);
                }
                else
                {
                    AllMatrix.Add(floorMashes[randomMeshIndex], new List<Matrix4x4>() { element });
                }
            }
        }

        foreach (var hallwayCell in Hallwaycell)
        {
            List<Matrix4x4> cellMatrix = new List<Matrix4x4>();
            List<Matrix4x4> cellFloorMatrix = new List<Matrix4x4>();

            cellMatrix.AddRange(hallwayCell.HallwayWallMatrix4x4(segmentSize));
            cellFloorMatrix.AddRange(hallwayCell.FlorMatrix4x4(segmentSize));
            foreach (var element in cellMatrix)
            {
                int randomMeshIndex = random.NextInt(0, hallwayMeshes.Count);


                if (AllMatrix.ContainsKey(hallwayMeshes[randomMeshIndex]))
                {
                    AllMatrix[hallwayMeshes[randomMeshIndex]].Add(element);
                }
                else
                {
                    AllMatrix.Add(hallwayMeshes[randomMeshIndex], new List<Matrix4x4>() { element });
                }
            }

            foreach (var element in cellFloorMatrix)
            {
                int randomMeshIndex = random.NextInt(0, floorMashes.Count);


                if (AllMatrix.ContainsKey(floorMashes[randomMeshIndex]))
                {
                    AllMatrix[floorMashes[randomMeshIndex]].Add(element);
                }
                else
                {
                    AllMatrix.Add(floorMashes[randomMeshIndex], new List<Matrix4x4>() { element });
                }
            }
        }
    }

    public void DrawWalls()
    {
        foreach (var element in AllMatrix)
        {
            Graphics.DrawMeshInstanced(element.Key, 0, meshesMaterial, element.Value.ToArray());
        }
    }

    Mesh CombineMeshesFromDictionary(Dictionary<Mesh, List<Matrix4x4>> meshDictionary)
    {
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (var kvp in meshDictionary)
        {
            Mesh mesh = kvp.Key;
            List<Matrix4x4> matrices = kvp.Value;

            Debug.Log($"Processing Mesh: {mesh.name}, Matrices Count={matrices.Count}");

            // Sprawdzanie, czy mesh zawiera dane wierzchołków i trójkątów
            Debug.Log($"Mesh {mesh.name} Vertices: {mesh.vertexCount}, Triangles: {mesh.triangles.Length / 3}");

            // Przechodzimy przez każdą macierz transformacji
            foreach (Matrix4x4 matrix in matrices)
            {
                // Sprawdzanie, czy mesh jest pusty
                if (mesh.vertexCount == 0 || mesh.triangles.Length == 0)
                {
                    Debug.LogWarning($"Mesh {mesh.name} is empty or invalid. Skipping this mesh.");
                    continue; // Pomijamy pusty mesh
                }

                CombineInstance instance = new CombineInstance
                {
                    mesh = mesh,
                    transform = matrix
                };

                Debug.Log($"Adding CombineInstance: Mesh={mesh.name}, Vertices={mesh.vertexCount}, Transform={matrix}");

                combineInstances.Add(instance);
            }
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogError("No CombineInstances were added!");
        }

        // Tworzenie zbiorczej siatki
        Mesh combinedMesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 // Obsługa dużych siatek
        };

        // Łączenie siatek
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        // Debugowanie liczby wierzchołków i trójkątów po połączeniu
        Debug.Log($"Combined Mesh: Vertices={combinedMesh.vertexCount}, Triangles={combinedMesh.triangles.Length / 3}");

        return combinedMesh;
    }

    void CreateIndividualMeshesFromDictionary(Dictionary<Mesh, List<Matrix4x4>> meshDictionary)
    {
        foreach (var kvp in meshDictionary)
        {
            Mesh mesh = kvp.Key;
            List<Matrix4x4> matrices = kvp.Value;

            Debug.Log($"Processing Mesh: {mesh.name}, Matrices Count={matrices.Count}");

            // Sprawdzanie, czy mesh zawiera dane wierzchołków i trójkątów
            Debug.Log($"Mesh {mesh.name} Vertices: {mesh.vertexCount}, Triangles: {mesh.triangles.Length / 3}");

            // Przechodzimy przez każdą macierz transformacji
            foreach (Matrix4x4 matrix in matrices)
            {
                // Sprawdzanie, czy mesh jest pusty
                if (mesh.vertexCount == 0 || mesh.triangles.Length == 0)
                {
                    Debug.LogWarning($"Mesh {mesh.name} is empty or invalid. Skipping this mesh.");
                    continue; // Pomijamy pusty mesh
                }

                // Tworzymy nowy GameObject dla każdej kombinacji
                GameObject meshObject = new GameObject($"{mesh.name}_Instance");
                MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

                // Ustawiamy mesh w MeshFilterze
                meshFilter.mesh = mesh;

                // Ustawiamy macierz transformacji na obiekt
                meshObject.transform.localPosition = matrix.GetColumn(3); // Translacja (pozycja)
                meshObject.transform.localRotation = matrix.rotation; // Rotacja
                meshObject.transform.localScale = matrix.lossyScale; // Skalowanie

                // Można dodać material, jeśli jest potrzebny
                // meshRenderer.material = yourMaterial;

                Debug.Log($"Created Mesh Object: {meshObject.name}, Transform={matrix}");
            }
        }
    }


    public void GenerateMeshes()
    {
        // Tworzymy Holder jako obiekt nadrzędny
        Transform holder = generatedRoomTransform;

        foreach (Room room in RoomGanerateSetting.CreatedRoom)
        {
            // Przechodzimy przez wszystkie macierze w room
            List<Matrix4x4> normalWallCell = new List<Matrix4x4>();
            List<Matrix4x4> passWallCell = new List<Matrix4x4>();
            List<Matrix4x4> floorCell = new List<Matrix4x4>();

            foreach (var roomCell in room.CellInRoom)
            {
                normalWallCell.AddRange(roomCell.RoomWallMatrix4s4(segmentSize));
                floorCell.AddRange(roomCell.FlorMatrix4x4(segmentSize));
            }

            foreach (var roomCell in room.CellInRoom)
            {
                passWallCell.AddRange(roomCell.RoomPassWallMatrix4x4(segmentSize));
            }

            // Spawnujemy pojedyncze meshe dla ścian
            SpawnMeshesFromMatrix(passWallCell, passesMeshes, holder, passableLayerMask, passThrow: true);
            SpawnMeshesFromMatrix(normalWallCell, wallMeshes, holder, wallLayerMask, true);
            SpawnMeshesFromMatrix(floorCell, floorMashes, holder, floorLayerMask, false, false, true);
        }

        foreach (var hallwayCell in Hallwaycell)
        {
            List<Matrix4x4> cellMatrix = new List<Matrix4x4>();
            List<Matrix4x4> cellFloorMatrix = new List<Matrix4x4>();

            cellMatrix.AddRange(hallwayCell.HallwayWallMatrix4x4(segmentSize));
            cellFloorMatrix.AddRange(hallwayCell.FlorMatrix4x4(segmentSize));

            // Spawnujemy pojedyncze meshe dla korytarzy
            SpawnMeshesFromMatrix(cellMatrix, hallwayMeshes, holder, wallLayerMask, true);
            SpawnMeshesFromMatrix(cellFloorMatrix, floorMashes, holder, floorLayerMask, false, false, true);
        }
    }

    private void SpawnMeshesFromMatrix(List<Matrix4x4> matrices, List<Mesh> meshPool, Transform parent,
        LayerMask layerToSet, bool isObstacle = false, bool passThrow = false, bool isFloor = false)
    {
        foreach (var matrix in matrices)
        {
            // Wybieramy losowy mesh z puli
            int randomMeshIndex = Random.Range(0, meshPool.Count);
            Mesh randomMesh = meshPool[randomMeshIndex];

            // Tworzymy nowy obiekt
            GameObject meshObject = new GameObject(randomMesh.name);
            meshObject.layer = Mathf.RoundToInt(Mathf.Log(layerToSet.value, 2));

            meshObject.transform.SetParent(parent);
            meshObject.transform.position = matrix.GetColumn(3);
            meshObject.transform.rotation =
                Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
            meshObject.transform.localScale = Vector3.one;

            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            meshFilter.mesh = randomMesh;

            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();


            if (passThrow)
            {
                meshRenderer.material = passesMaterial;
            }
            else
            {
                meshRenderer.material = meshesMaterial;
            }

            if (!passThrow)
            {
                if (isFloor)
                {
                    meshObject.AddComponent<MeshCollider>();
                }
                else
                {
                    var boxCollider = meshObject.AddComponent<BoxCollider>();
                    boxCollider.center = randomMesh.bounds.center;
                    boxCollider.size = randomMesh.bounds.size;
                }

                if (isObstacle)
                {
                    var obstacle = meshObject.AddComponent<NavMeshObstacle>();
                    obstacle.carving = true;


                    obstacle.shape = NavMeshObstacleShape.Box;
                }
            }
        }
    }


    /// <summary>
    /// Wypiekanie NavMesh.
    /// </summary>
    public void BakeNavigation()
    {
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface nie jest przypisany! Upewnij się, że został dodany do obiektu.");
            return;
        }

        try
        {
            Debug.Log("Rozpoczynanie generowania NavMesh...");
            BakeNavMesh();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Błąd podczas generowania NavMesh: {ex.Message}");
        }
    }

    /// <summary>
    /// Metoda pomocnicza do generowania NavMesh.
    /// </summary>
    private void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.collectObjects = CollectObjects.Children;
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh został pomyślnie wygenerowany!");
        }
        else
        {
            Debug.LogWarning("NavMeshSurface jest pusty, nie można wygenerować NavMesh.");
        }
    }
}
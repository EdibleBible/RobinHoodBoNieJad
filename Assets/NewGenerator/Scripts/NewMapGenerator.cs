using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomGrid;
using NUnit.Framework;
using Unity.AI.Navigation;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class NavMeshSurfaceSettings
{
    public NavMeshSurface NavMeshSurfaceWalkable;
    public GameObject EnemyPrefab;

    public void BakeNavMes()
    {
        NavMeshSurfaceWalkable.BuildNavMesh();
    }

    public void SpawnEnemy(MonoBehaviour context, Grid<GridCellData> grid)
    {
        context.StartCoroutine(SpawnEnemyCoroutine(grid));
    }

    private IEnumerator SpawnEnemyCoroutine(Grid<GridCellData> grid)
    {
        Vector3 center = new Vector3((grid.GetCellSize() * grid.GetWidth()) / 2, 0,
            (grid.GetCellSize() * grid.GetHeight()) / 2);

        float mapRadius = Mathf.Max(grid.GetWidth(), grid.GetHeight()) * grid.GetCellSize() / 2f;

        bool found = false;
        Vector3 spawnPoint = Vector3.zero;

        int areaIndex = NavMesh.GetAreaFromName("Walkable");
        if (areaIndex < 0)
        {
            Debug.LogWarning("NavMesh area 'Walkable' not found.");
            yield break;
        }

        int areaMask = 1 << areaIndex;

        while (!found)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * mapRadius;
            randomPoint.y = center.y;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.0f, areaMask))
            {
                spawnPoint = hit.position;
                found = true;
            }
            else
            {
                yield return null; // poczekaj 1 frame i próbuj dalej
            }
        }

        GameObject.Instantiate(EnemyPrefab, spawnPoint, Quaternion.identity);
    }
}

public class NewMapGenerator : MonoBehaviour
{
    private Unity.Mathematics.Random random;
    public bool DebugGridMeshBool;
    public bool DebugRemoveMesh;


    public CustomGrid.Grid<GridCellData> CreatedGrid;
    public GridParameters GridParameters;
    public RoomGeneratorSettings RoomGeneratorSettings;
    public MeshGeneratorSettings MeshGeneratorSettings;
    public SpawnPlayerSettings SpawnPlayerSettings;
    public SpawnTrapSettings SpawnTrapSettings;
    public SpawnDoorVariableSettings SpawnDoorVariableSettings;
    public NavMeshSurfaceSettings NavMeshSurfaceSettings;


    public Dictionary<GridCellData, GameObject> allObject = new Dictionary<GridCellData, GameObject>();
    [SerializeField] private SOLevel levelData;

    [Header("Pathfinding")] private Pathfinding currentPathfinding;
    public DelaunayTriangulator Triangulator;
    public List<Point> AllPoints = new List<Point>();
    public List<Edge> AllEdges = new List<Edge>();
    public List<Edge> SelectedEdges = new List<Edge>();
    public List<Triangle> AllTriangles = new List<Triangle>();
    public bool AvaibleDifferentRoomsOnly;
    private bool playerIsSpawn;

    private void Awake()
    {
        try
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

            AllTriangles = GenerateTraingulationBeetweenCells();

            GetSelectedEdges();

            CreatePathfindingForSelectedEdges();

            GenerateHallway();

            ControllSpawners();

            foreach (var room in RoomGeneratorSettings.AllCreatedRooms)
            {
                room.SetInteractableObjectNullParent();
            }

            SpawnBlockedDoors();

            SpawnTraps();

            SpawnDoorVariableSettings.RandomizeDoors();
            SpawnDoorVariableSettings.SelectLeverGridCell(MeshGeneratorSettings.GetShuffledHallwayCells());
            SpawnDoorVariableSettings.AssignLeversToDoors();

            NavMeshSurfaceSettings.BakeNavMes();
            NavMeshSurfaceSettings.SpawnEnemy(this, CreatedGrid);

            SpawnPlayer();
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Awake() failed: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            if (DebugGridMeshBool)
                DebugGridMesh();
            
            if (DebugRemoveMesh)
            {
                foreach (Transform child in RoomGeneratorSettings.roomParent)
                {
                    DestroyImmediate(child.gameObject);
                }

                foreach (Transform child in MeshGeneratorSettings.MeshesParent)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }

    private void Update()
    {
        if (!playerIsSpawn && Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPlayer();
        }
    }

    private void SpawnBlockedDoors()
    {
        List<GeneratorRoomData> data = new List<GeneratorRoomData>();

        foreach (var room in RoomGeneratorSettings.AllCreatedRooms)
        {
            data.Add(room.RoomData);
        }

        SpawnDoorVariableSettings.FindAllDoorsOnScene(data);
        SpawnDoorVariableSettings.RandomizeDoorsAmount(levelData.LevelSeedUint);
    }

    private void SpawnTraps()
    {
        int trapsCount = 0;

        foreach (var cell in MeshGeneratorSettings.GetShuffledHallwayCells())
        {
            int randomInt = random.NextInt(0, 101);
            if (randomInt < SpawnTrapSettings.ChanceToSpawn)
            {
                if (trapsCount >= SpawnTrapSettings.MaxTrapsCount)
                    return;

                trapsCount++;
                int trapIndex = random.NextInt(SpawnTrapSettings.TrapsPrefabs.Count);
                Instantiate(SpawnTrapSettings.TrapsPrefabs[trapIndex], cell.Position, quaternion.identity);
                cell.GridCellType = E_GridCellType.HallwayTrap;
            }
        }
    }

    public void SpawnPlayer()
    {
        Debug.Log("SPAWN");

        GameObject spawnRoom = RoomGeneratorSettings.AllCreatedRooms.Where(x => x.IsSpawn)
            .Select(x => x.SpawnedRoomObject).FirstOrDefault();
        GeneratorRoomData roomData = spawnRoom.GetComponent<GeneratorRoomData>();


        Instantiate(SpawnPlayerSettings.CanvasPrefab);

        if (roomData.SpawnPosition == null)
        {
            Instantiate(SpawnPlayerSettings.PlayerPrefab,
                spawnRoom.transform.position + SpawnPlayerSettings.SpawnOffset,
                Quaternion.identity);
            playerIsSpawn = true;
        }
        else
        {
            Instantiate(SpawnPlayerSettings.PlayerPrefab,
                roomData.SpawnPosition.position + SpawnPlayerSettings.SpawnOffset,
                Quaternion.identity);
            playerIsSpawn = true;
        }

        var controler = GameController.Instance;
        controler.ToogleCursorOff(false);
        controler.ToggleFullScreenPass(true);
    }

    private void CreatePathfindingForSelectedEdges()
    {
        List<GridCellData> roomCells = new List<GridCellData>();
        List<GridCellData> allCells = CreatedGrid.GetAllGridElementList();

        foreach (var room in RoomGeneratorSettings.AllCreatedRooms)
        {
            roomCells.AddRange(room.CellInRoom);
            foreach (var cell in room.CellInRoom)
            {
                if (cell.GridCellType != E_GridCellType.Pass)
                    continue;

                cell.DetectObjectsInCell(RoomGeneratorSettings.DetectObjectInCellLayerMask);
            }
        }

        currentPathfinding = new Pathfinding(
            CreatedGrid.GetWidth(),
            CreatedGrid.GetHeight(),
            CreatedGrid.GetCellSize(),
            transform.position,
            roomCells,
            allCells);


        foreach (var edge in SelectedEdges)
        {
            List<PathNode> path = new List<PathNode>();
            GridCellData startCell = CreatedGrid.GetValue((int)edge.Point1.X / (int)CreatedGrid.GetCellSize(),
                (int)edge.Point1.Y / (int)CreatedGrid.GetCellSize());
            GridCellData endCell = CreatedGrid.GetValue((int)edge.Point2.X / (int)CreatedGrid.GetCellSize(),
                (int)edge.Point2.Y / (int)CreatedGrid.GetCellSize());
            ;

            Vector2Int startCoordinate = startCell.GetDoorExitCell().Coordinate;
            Vector2Int endCoordinate = endCell.GetDoorExitCell().Coordinate;

            path = currentPathfinding.FindPath(startCoordinate.x, startCoordinate.y, endCoordinate.x, endCoordinate.y);

            startCell.GetDoorExitCell().GridCellType = E_GridCellType.HallwayCorner;
            endCell.GetDoorExitCell().GridCellType = E_GridCellType.HallwayCorner;
            
            foreach (var node in path)
            {
                GridCellData toAdd = CreatedGrid.GetValue(node.X, node.Y);
                toAdd.GridCellType = E_GridCellType.Hallway;

                if (!MeshGeneratorSettings.HallwayCell.Contains(toAdd))
                    MeshGeneratorSettings.HallwayCell.Add(toAdd);
            }

            startCell.GetDoorExitCell().GridCellType = E_GridCellType.HallwayBorder;
            endCell.GetDoorExitCell().GridCellType = E_GridCellType.HallwayBorder;
        }
    }

    private void GetSelectedEdges()
    {
        List<Edge> selectedEdges = PrimAlgorithm.FindMST(AllEdges, AllPoints);
        SelectedEdges.AddRange(selectedEdges);

        int additionalEdges = 0;
        HashSet<Vector2Int> mergetedPoints = new HashSet<Vector2Int>();

        if (RoomGeneratorSettings.UseAdditionalEdges)
        {
            foreach (var edge in AllEdges)
            {
                if (additionalEdges >= RoomGeneratorSettings.AdditionSelectedEdges)
                    break;

                if (selectedEdges.Contains(edge))
                    continue;

                var randomNumber = random.NextInt(0, 101);

                if (randomNumber > RoomGeneratorSettings.ChanceToSelectEdge)
                    continue;

                SelectedEdges.Add(edge);
                additionalEdges++;
            }
        }
    }

    private List<Triangle> GenerateTraingulationBeetweenCells()
    {
        List<Triangle> allTriangles = new List<Triangle>();
        Triangulator = new DelaunayTriangulator();
        Dictionary<object, Point> points = new Dictionary<object, Point>();

        foreach (var cell in SelectPassCellFromRooms())
        {
            Point newPoint = new Point(cell.Position.x, cell.Position.z);
            points.Add(cell, newPoint);
        }

        allTriangles = Triangulator.BowyerWatson(points).ToList();

        AllEdges = GetEdgesFromTriangles(allTriangles);
        AllPoints = GetPointsFromTriangles(allTriangles);

        return allTriangles;
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

    private List<GridCellData> SelectPassCellFromRooms()
    {
        List<GridCellData> allPassCells = new List<GridCellData>();
        foreach (var room in RoomGeneratorSettings.AllCreatedRooms)
        {
            allPassCells.AddRange(room.GetPassGridCell());
        }

        foreach (var cell in allPassCells)
        {
            Debug.Log($"cell pss cords = x:{cell.Coordinate.x}, y:{cell.Coordinate.y}");
        }

        return allPassCells;
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

    private void GenerateHallway()
    {
        List<Matrix4x4> wallMatrix = new List<Matrix4x4>();
        List<Matrix4x4> floorMatrix = new List<Matrix4x4>();

        foreach (var cell in MeshGeneratorSettings.HallwayCell)
        {
            List<Matrix4x4> toAddWall = cell.RoomWallMatrix4s4(MeshGeneratorSettings.WallSegmentSize);
            List<Matrix4x4> toAddFloor = cell.FlorMatrix4x4(MeshGeneratorSettings.FloorSegmentSize);

            wallMatrix.AddRange(toAddWall);
            floorMatrix.AddRange(toAddFloor);
        }

        SpawnMeshesFromMatrix(wallMatrix, MeshGeneratorSettings.WallMeshes, MeshGeneratorSettings.MeshesParent,
            MeshGeneratorSettings.WallLayerMask);
        SpawnMeshesFromMatrix(floorMatrix, MeshGeneratorSettings.FloorMeshes, MeshGeneratorSettings.MeshesParent,
            MeshGeneratorSettings.FloorLayerMask, true);
    }

    private void SpawnMeshesFromMatrix(List<Matrix4x4> matrices, List<Mesh> meshPool, Transform parent,
        LayerMask layerToSet, bool isFloor = false)
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


            if (isFloor)
            {
                meshRenderer.material = MeshGeneratorSettings.FloorMaterial;
                meshObject.AddComponent<MeshCollider>();
            }
            else
            {
                meshRenderer.material = MeshGeneratorSettings.WallMaterial;
                var boxCollider = meshObject.AddComponent<BoxCollider>();
                boxCollider.center = randomMesh.bounds.center;
                boxCollider.size = randomMesh.bounds.size;
            }
        }
    }

    private void ControllSpawners()
    {
        List<RandomItemGenerator> spawners =
            FindObjectsByType<RandomItemGenerator>(FindObjectsSortMode.InstanceID).ToList();

        foreach (var spawner in spawners)
        {
            Debug.Log("Spawner");
            spawner.SetupSpawner(GameController.Instance);
            spawner.ControllSpawner();
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
                var createdCell =
                    new GameObject(
                        $"cell nr x:{cell.Coordinate.x} y:{cell.Coordinate.y} cell position:{cell.Position}:");
                createdCell.transform.parent = transform;

                createdCell.transform.position = cell.Position;

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
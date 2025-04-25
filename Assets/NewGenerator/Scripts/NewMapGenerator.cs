using System;
using System.Collections.Generic;
using System.Linq;
using CustomGrid;
using UnityEngine;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

public class NewMapGenerator : MonoBehaviour
{
    public CustomGrid.Grid<GridCellData> CreatedGrid;
    public GridParameters GridParameters;
    public RoomGeneratorSettings RoomGeneratorSettings;


    public Dictionary<GridCellData, GameObject> allObject = new Dictionary<GridCellData, GameObject>();
    [SerializeField] private SOLevel levelData;

    private void Awake()
    {
        if (GridParameters.IsRandomized)
        {
            RandimizeSeed();
            GenerateGrid(levelData.LevelSeedUint);
        }
        else
        {
            GenerateGrid(GridParameters.GridSize.x, GridParameters.GridSize.y);
        }

        RoomGeneratorSettings.CreateRoomOnGrid(CreatedGrid, levelData);
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

    private void OnDrawGizmos()
    {
        Color color = Color.white;
        foreach (var room in RoomGeneratorSettings.AllCreatedRooms)
        {
            Gizmos.DrawSphere(room.Centroid + new Vector3(0, 3, 0), 0.2f);
            Gizmos.DrawLine(room.Centroid, room.Centroid + new Vector3(0, 3, 0));
        }
    }
}

[Serializable]
public struct GridParameters
{
    public bool IsRandomized;

    public Vector2Int GridSize;
    public Vector2Int MaxGridSize;
    public Vector2Int MinGridSize;

    public float CellSize;
    public AYellowpaper.SerializedCollections.SerializedDictionary<E_GridCellType, Material> materials;

    public void RandomizeGridSize(uint seed)
    {
        Random random = new Random(seed);

        if (IsRandomized)
        {
            int x = random.NextInt(MinGridSize.x, MaxGridSize.x);
            int y = random.NextInt(MinGridSize.y, MaxGridSize.y);

            GridSize = new Vector2Int(x, y);
        }
    }
}

[Serializable]
public struct RoomGeneratorSettings
{
    public Transform roomParent;
    public Vector2Int MaxRoomSize;
    public Vector2Int MinRoomSize;
    public int RoomCount;
    public int MinRoomDistance;
    public int MaxRoomDistance;
    public int MaxAttempts;
    public List<NewRoom> AllCreatedRooms;
    public LayerMask WallPassLayer;

    public void CreateRoomOnGrid(Grid<GridCellData> createdGrid, SOLevel levelSeed)
    {
        Random random = new Random(levelSeed.LevelSeedUint);
        List<NewRoom> createdRooms = new List<NewRoom>();

        int attempts = 0;

        for (int i = 0; i < RoomCount; i++)
        {
            attempts = 0;
            NewRoom createdRoom = null;

            while (attempts < MaxAttempts && createdRoom == null)
            {
                createdRoom = GenerateRoom(ref random, createdGrid);
                attempts++;
            }

            if (createdRoom == null)
            {
                Debug.LogWarning("Unable to generate room after maximum attempts.");
                continue;
            }

            createdRooms.Add(createdRoom);
        }

        AllCreatedRooms = createdRooms;

        for (int i = 0; i < AllCreatedRooms.Count; i++)
        {
            AllCreatedRooms[i].RoomID = i;
        }
    }

    private NewRoom GenerateRoom(ref Random random, Grid<GridCellData> createdGrid)
    {
        NewRoom room = new NewRoom();
        int width = (MinRoomSize.x == MaxRoomSize.x) ? MinRoomSize.x : random.NextInt(MinRoomSize.x, MaxRoomSize.x + 1);
        int height = (MinRoomSize.y == MaxRoomSize.y)
            ? MinRoomSize.y
            : random.NextInt(MinRoomSize.y, MaxRoomSize.y + 1);

        int startX = random.NextInt(0, createdGrid.GeneratedGridSize().x - width);
        int startY = random.NextInt(0, createdGrid.GeneratedGridSize().y - height);

        if (!CanplaceRoom(startX, startY, width, height, createdGrid))
        {
            return null;
        }

        for (int i = startX; i < startX + width; i++)
        {
            for (int j = startY; j < startY + height; j++)
            {
                GridCellData selectedCell = createdGrid.GetValue(i, j);
                selectedCell.GridCellType = E_GridCellType.RoomInside;
                room.CellInRoom.Add(selectedCell);
            }
        }


        room.XAxisSize = width;
        room.YAxisSize = height;
        room.RoomParent = roomParent;
        room.CalculateRoomCentroid(createdGrid.GetCellSize());
        room.SpawnPrefabs(LayerMaskToLayer(WallPassLayer));
        return room;
    }

    private bool CanplaceRoom(int startX, int startY, int width, int height, Grid<GridCellData> gridData)
    {
        if (gridData.GetValue(startX, startY).GridCellType != E_GridCellType.Empty)
        {
            return false;
        }

        for (int x = startX - MinRoomDistance; x < startX + width + MinRoomDistance; x++)
        {
            for (int y = startY - MinRoomDistance; y < startY + height + MinRoomDistance; y++)
            {
                GridCellData cellData = gridData.GetValue(x, y);
                if (cellData != null && cellData.GridCellType != E_GridCellType.Empty)
                    return false;

                if (x == 0 || y == 0 || x == gridData.GetWidth() - 1 || y == gridData.GetHeight() - 1)
                    return false;
            }
        }

        return true;
    }

    private int LayerMaskToLayer(LayerMask mask)
    {
        int value = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((value & (1 << i)) != 0)
                return i;
        }

        Debug.LogWarning("LayerMask contains multiple layers, using the first found.");
        return 0;
    }
}

[Serializable]
public class NewRoom
{
    public E_RoomType RoomType;
    public int RoomID;
    public List<GridCellData> CellInRoom = new List<GridCellData>();
    public int XAxisSize;
    public int YAxisSize;
    public Transform RoomParent;

    public Vector3 Centroid = new Vector3(0, 0, 0);

    public void CalculateRoomCentroid(float _cellSize)
    {
        float totalX = 0f;
        float totalY = 0f;

        foreach (var cell in CellInRoom)
        {
            float centerX = cell.Coordinate.x * _cellSize + _cellSize / 2f;
            float centerY = cell.Coordinate.y * _cellSize + _cellSize / 2f;

            totalX += centerX;
            totalY += centerY;
        }

        Centroid = new Vector3(totalX / CellInRoom.Count(), 0, totalY / CellInRoom.Count());
    }

    public Vector3 GetRoomCentroid()
    {
        return Centroid;
    }

    public void SpawnPrefabs(int wallPassLayerInt)
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>($"RoomsPrefab/{YAxisSize}x{XAxisSize}");
        if (prefabs.Length > 0)
        {
            // Wylosuj jeden
            int randomIndex = UnityEngine.Random.Range(0, prefabs.Length);
            GameObject selectedPrefab = prefabs[randomIndex];

            // Instantiate go np. w (0, 0, 0) z domyślną rotacją
            var obj = GameObject.Instantiate(selectedPrefab, GetRoomCentroid(), Quaternion.identity, RoomParent);
            obj.transform.position = Centroid;

            GeneratorRoomData roomData = obj.GetComponent<GeneratorRoomData>();
            Transform[] selectedFloors =
                roomData.AllFloors.Where(x => x.gameObject.layer == wallPassLayerInt).ToArray();

            foreach (var cell in CellInRoom)
            {
                Debug.Log($"cell position: {cell.Position}");
                if (selectedFloors.Any(x => x.position.x - 1 == cell.Position.x && x.position.z - 1 == cell.Position.z))
                {
                    Debug.Log("Cell type changed");
                    cell.GridCellType = E_GridCellType.Pass;
                }
            }
            /*foreach (var floor in selectedFloors)
            {
                selectedCells.AddRange(CellInRoom
                    .Where(x => x.Position.x == floor.position.x && x.Position.y == floor.position.y).ToList());
            }

            foreach (var cell in selectedCells)
            {
                cell.GridCellType = E_GridCellType.Pass;
            }*/
        }
        else
        {
            Debug.LogWarning("Brak prefabów w katalogu RoomsPrefab/1x1");
        }
    }

    private int LayerMaskToLayer(LayerMask mask)
    {
        int value = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((value & (1 << i)) != 0)
                return i;
        }

        Debug.LogWarning("LayerMask contains multiple layers, using the first found.");
        return 0;
    }
}
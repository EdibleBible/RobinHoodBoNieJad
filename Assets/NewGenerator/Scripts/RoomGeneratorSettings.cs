using System;
using System.Collections.Generic;
using CustomGrid;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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
    public LayerMask WallPassSpawnLayer;
    public bool UseAdditionalEdges;
    public float ChanceToSelectEdge;
    public int AdditionSelectedEdges;
    public LayerMask DetectObjectInCellLayerMask;



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

        SelectSpawn();


        for (int i = 0; i < AllCreatedRooms.Count; i++)
        {
            AllCreatedRooms[i].SpawnPrefabs(LayerMaskToLayer(WallPassLayer), LayerMaskToLayer(WallPassSpawnLayer));

            AllCreatedRooms[i].RoomID = i;
            foreach (var cell in AllCreatedRooms[i].CellInRoom)
            {
                cell.SetRoomID(i);
            }
        }
    }

    private void SelectSpawn()
    {
        int randomIndex = UnityEngine.Random.Range(0, AllCreatedRooms.Count);
        AllCreatedRooms[randomIndex].IsSpawn = true;
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
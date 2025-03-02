using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct RoomGanerateSetting
{
    [Header("BaseInfo")] public Vector2Int MaxRoomSize;
    public Vector2Int MinRoomSize;
    public int RoomCount;
    public int MinRoomDistance;
    public int MaxRoomDistance;
    public int MaxAttempts;

    [Header("Additional way")] public bool UseAdditionalEdges;
    [UnityEngine.Range(0, 25)] public int AdditionSelectedEdges;
    [UnityEngine.Range(0, 100)] public int ChanceToSelectEdge;

    [Header("Secret way")] public bool CreateSecretWay;
    [UnityEngine.Range(0, 5)] public int SecretWayMaxCount;

    [Header("Rooms")] public List<Room> CreatedRoom;

    [Header("Inroom Generator")] public GameObject PlayerPrefab;
    public List<GameObject> ObjectToPickList;
    public GameObject DepositPointPrefab;
    public LayerMask InroomObjectLayer;

    [Header("Door Spawn")] public GameObject DoorPrefab;
    public float DoorSpawnOffset;

    public void CreateRoomsOnGrid(CustomGrid.Grid<GridCellData> generatedGrid, uint seed, Transform roomTransform)
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);
        CreatedRoom = new List<Room>();
        int attempt = 0;


        for (int i = 0; i < RoomCount;)
        {
            Room room = null;
            if (i == 0)
                room = GenerateRoom(generatedGrid, random, ref attempt, true);
            else
                room = GenerateRoom(generatedGrid, random, ref attempt, false);


            // Jeśli po kilku próbach nie udało się stworzyć pokoju, przerywamy
            i++;

            if (room == null)
            {
                Debug.LogWarning("Unable to generate room after maximum attempts.");
                continue; // Jeśli nie udało się wygenerować pokoju, przerwij
            }


            if (room.CellInRoom.Count > 0)
            {
                room.RoomID = i;
                room.centroid = room.RoomCentroid(generatedGrid.GetCellSize());
                room.MarkCorners();
                CreatedRoom.Add(room);
            }

            GameObject roomParent = new GameObject("Room: " + i);
            roomParent.transform.SetParent(roomTransform);

            room.SetUpParent(roomParent, generatedGrid.GetCellSize());

        }

        foreach (var room in CreatedRoom)
        {
            room.SetRoomAssigned();
        }

        foreach (var room in CreatedRoom)
        {
            room.ChcekRoomContact(CreatedRoom);
        }
    }
    
    private Room GenerateRoom(CustomGrid.Grid<GridCellData> gridData, Unity.Mathematics.Random random, ref int attempt,
        bool isFirstRoom)
    {
        Room room = new Room();
        room.CellInRoom = new List<GridCellData>();

        int localAttempts = 0;
        bool roomIsGenerated = false;

        while (!roomIsGenerated && localAttempts < MaxAttempts)
        {
            localAttempts++;
            attempt++;

            // Jeśli Min i Max Room Size są równe, nie losujemy, tylko przypisujemy wartość
            int width = (MinRoomSize.x == MaxRoomSize.x) ? MinRoomSize.x : random.NextInt(MinRoomSize.x, MaxRoomSize.x + 1);
            int height = (MinRoomSize.y == MaxRoomSize.y) ? MinRoomSize.y : random.NextInt(MinRoomSize.y, MaxRoomSize.y + 1);

            // Losuj pozycję startową
            int x = random.NextInt(0, gridData.GeneratedGridSize().x - width);
            int y = random.NextInt(0, gridData.GeneratedGridSize().y - height);

            // Sprawdź warunki dla pierwszego pokoju i kolejnych
            if (CanPlaceRoom(gridData, new Vector2Int(x, y), width, height) &&
                (isFirstRoom || MaxRoomDistance == 0 || IsWithinMaxDistanceOfAnyRoom(gridData, new Vector2Int(x, y), width, height)))
            {
                // Dodaj komórki pokoju do siatki
                for (int i = x; i < x + width; i++)
                {
                    for (int j = y; j < y + height; j++)
                    {
                        GridCellData selectedCell = gridData.GetValue(i, j);
                        selectedCell.GridCellType = E_GridCellType.Room;
                        room.CellInRoom.Add(selectedCell);
                    }
                }

                room.XAxisSize = width;
                room.YAxisSize = height;
                room.RoomType = E_RoomType.StandardRoom;

                roomIsGenerated = true; // Pokój został poprawnie wygenerowany
            }
        }

        return roomIsGenerated ? room : null; // Zwróć pokój, jeśli został wygenerowany, w przeciwnym razie null
    }
    
    private bool CanPlaceRoom(CustomGrid.Grid<GridCellData> gridData, Vector2Int start, int width, int height)
    {
        for (int x = start.x - MinRoomDistance; x < start.x + width + MinRoomDistance; x++)
        {
            for (int y = start.y - MinRoomDistance; y < start.y + height + MinRoomDistance; y++)
            {
                if (x < 0 || y < 0 || x >= gridData.GeneratedGridSize().x || y >= gridData.GeneratedGridSize().y)
                    continue;

                GridCellData cell = gridData.GetValue(x, y);

                // Jeśli komórka jest już zajęta przez pokój lub dotyka krawędzi mapy, odrzuć
                if (cell != null && cell.GridCellType == E_GridCellType.Room ||
                    (x == 0 || y == 0 || x == gridData.GetGridArray().GetLength(0) - 1 ||
                     y == gridData.GetGridArray().GetLength(1) - 1))
                    return false;
            }
        }

        return true;
    }
    
    private bool IsWithinMaxDistanceOfAnyRoom(CustomGrid.Grid<GridCellData> gridData, Vector2Int start, int width,
        int height)
    {
        if (MaxRoomDistance == 0) return true; // Jeśli max dystans to 0, zawsze zwracaj true

        for (int x = start.x - MaxRoomDistance; x < start.x + width + MaxRoomDistance; x++)
        {
            for (int y = start.y - MaxRoomDistance; y < start.y + height + MaxRoomDistance; y++)
            {
                if (x < 0 || y < 0 || x >= gridData.GeneratedGridSize().x || y >= gridData.GeneratedGridSize().y)
                    continue;

                GridCellData cell = gridData.GetValue(x, y);

                // Jeśli znaleziono przynajmniej jeden pokój w promieniu MaxRoomDistance, zwróć true
                if (cell != null && cell.GridCellType == E_GridCellType.Room)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public void MakeTraingulateBetweenRoom()
    {
        List<Vector3> roomCentroid = CreatedRoom.Select(room => room.centroid).ToList();
    }
    
    public void SpawnPlayer()
    {
        foreach (var room in CreatedRoom)
        {
            room.SpawnPlayer(PlayerPrefab, DepositPointPrefab);
            break;
        }
    }
    
    public void SpawnRoomInside()
    {
        foreach (var room in CreatedRoom)
        {
            room.GenerateRoomInside();
        }
    }
    
    public void DetectObjects()
    {
        foreach (var room in CreatedRoom)
        {
            room.DetectObjects(InroomObjectLayer);
        }
    }
    
    public void SpawnDoor()
    {
        foreach (var room in CreatedRoom)
        {
            room.SpawnDoors(DoorPrefab, DoorSpawnOffset);
        }
    }
}
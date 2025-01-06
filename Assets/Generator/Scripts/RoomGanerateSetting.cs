using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public struct RoomGanerateSetting
{
    [Header("BaseInfo")] public Vector2Int MaxRoomSize;
    public Vector2Int MinRoomSize;
    public int RoomCount;
    public int MinRoomDistance;
    public int MaxAttempts;

    [Header("Additional way")] public bool UseAdditionalEdges;
    [Range(0, 25)] public int AdditionSelectedEdges;
    [Range(0, 100)] public int ChanceToSelectEdge;

    [Header("Secret way")] public bool CreateSecretWay;
    [Range(0, 5)] public int SecretWayMaxCount;

    [Header("Rooms")] public List<Room> CreatedRoom;

    [Header("Inroom Generator")] public GameObject playerPrefab;

    public void CreateRoomsOnGrid(CustomGrid.Grid<GridCellData> generatedGrid, uint seed)
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);
        CreatedRoom = new List<Room>();
        int attempt = 0;

        for (int i = 0; i < RoomCount;)
        {
            i++;
            // Próba wygenerowania pokoju
            Room room = GenerateRoom(generatedGrid, random, ref attempt);
            // Jeśli po kilku próbach nie udało się stworzyć pokoju, przerywamy

            if (room == null)
            {
                Debug.LogWarning("Unable to generate room after maximum attempts.");
                continue; // Jeśli nie udało się wygenerować pokoju, przerwij
            }

            if (room.CellInRoom.Count > 0)
            {
                room.RoomID = i;
                room.cetroid = room.RoomCentroid();
                room.MarkCorners();
                CreatedRoom.Add(room);
            }
        }
    }

    private Room GenerateRoom(CustomGrid.Grid<GridCellData> gridData, Unity.Mathematics.Random random, ref int attempt)
    {
        Room room = new Room();
        room.CellInRoom = new List<GridCellData>();

        int localAttempts = 0;
        bool roomIsGenerated = false;

        while (!roomIsGenerated && localAttempts < MaxAttempts)
        {
            localAttempts++;
            attempt++;

            // Losuj rozmiar pokoju
            int width = random.NextInt(MinRoomSize.x, MaxRoomSize.x + 1);
            int height = random.NextInt(MinRoomSize.y, MaxRoomSize.y + 1);

            // Losuj pozycję startową
            int x = random.NextInt(0, gridData.GeneratedGridSize().x - width);
            int y = random.NextInt(0, gridData.GeneratedGridSize().y - height);

            // Sprawdź, czy pokój można umieścić
            if (CanPlaceRoom(gridData, new Vector2Int(x, y), width, height))
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

    // Funkcja walidująca, czy pokój zmieści się w siatce
    private bool CanPlaceRoom(CustomGrid.Grid<GridCellData> gridData, Vector2Int start, int width, int height)
    {
        for (int x = start.x - MinRoomDistance; x < start.x + width + MinRoomDistance; x++)
        {
            for (int y = start.y - MinRoomDistance; y < start.y + height + MinRoomDistance; y++)
            {
                if (x < 0 || y < 0 || x >= gridData.GeneratedGridSize().x || y >= gridData.GeneratedGridSize().y)
                    continue;

                GridCellData cell = gridData.GetValue(x, y);

                // Jeśli komórka jest już zajęta przez pomieszczenie, nie można tu utworzyć pokoju
                if (cell != null && cell.GridCellType == E_GridCellType.Room || (x == 0 || y == 0 ||
                        x == gridData.GetGridArray().GetLength(0) - 1 || y == gridData.GetGridArray().GetLength(1) - 1))
                    return false;
            }
        }

        return true;
    }

    public void MakeTraingulateBetweenRoom()
    {
        List<Vector3> roomCentroid = CreatedRoom.Select(room => room.RoomCentroid()).ToList();
    }

    public void SpawnPlayer()
    {
        foreach (var room in CreatedRoom)
        {
            room.SpawnPlayer(playerPrefab);
            break;
        }
    }
}
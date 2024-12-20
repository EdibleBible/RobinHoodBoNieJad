using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public struct RoomGanerateSetting
{
    [Header("BaseInfo")]
    public Vector2Int MaxRoomSize;
    public Vector2Int MinRoomSize;
    public int RoomCount;
    public int MinRoomDistance;
    public int MaxAttempts;

    [Header("Additional way")]
    public bool UseAdditionalEdges;
    [Range(0, 25)] public int AdditionSelectedEdges;
    [Range(0, 100)] public int ChanceToSelectEdge;

    [Header("Secret way")]

    public bool CreateSecretWay;
    [Range(0, 5)] public int SecretWayMaxCount;

    [Header("Rooms")]
    public List<Room> CreatedRoom;


    public void CreateRoomsOnGrid(CustomGrid.Grid<GridCellData> generatedGrid, System.Random random)
    {
        CreatedRoom = new List<Room>();
        int attempt = 0;

        for (int i = 0; i < RoomCount;)
        {
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
                CreatedRoom.Add(room);
                Debug.Log($"Room: {room.RoomID} have {room.CellInRoom.Count}");

            }
            i++;
        }
    }

    private Room GenerateRoom(CustomGrid.Grid<GridCellData> gridData, System.Random random, ref int attempt)
    {
        attempt++;

        if (attempt > MaxAttempts)
        {
            // Jeśli osiągnięto limit prób, zwróć null
            return null;
        }

        // Tworzymy instancję randoma z seedem

        int width = random.Next(MinRoomSize.x, MaxRoomSize.x);
        int height = random.Next(MinRoomSize.y, MaxRoomSize.y);

        Room room = new Room();
        room.XAxisSize = width;
        room.YAxisSize = height;
        room.CellInRoom = new List<GridCellData>();

        bool roomIsGenerated = false;
        while (!roomIsGenerated)
        {
            int x = random.Next(0, gridData.GeneratedGridSize().x - width);
            int y = random.Next(0, gridData.GeneratedGridSize().y - height);

            if (CanPlaceRoom(gridData, new Vector2Int(x, y), width, height))
            {
                for (int i = x; i < x + width; i++)
                {
                    for (int j = y; j < y + height; j++)
                    {
                        GridCellData selectedCell = gridData.GetValue(i, j);
                        selectedCell.GridCellType = E_GridCellType.Room;
                        room.CellInRoom.Add(selectedCell);
                        room.RoomType = E_RoomType.StandardRoom;
                    }
                }
            }
            else if (attempt > MaxAttempts)
            {
                return null; // Jeśli próba przekroczyła maksymalną liczbę, zakończ generowanie
            }

            roomIsGenerated = true;
        }
        return room;
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
                if (cell != null && cell.GridCellType == E_GridCellType.Room)
                    return false;
            }
        }
        return true;
    }

    public void MakeTraingulateBetweenRoom()
    {
        List<Vector3> roomCentroid = CreatedRoom.Select(room => room.RoomCentroid()).ToList();
    }

}


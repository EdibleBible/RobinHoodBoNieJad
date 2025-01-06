using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class Room
{
    public E_RoomType RoomType;
    public int RoomID;
    public List<GridCellData> CellInRoom;
    public int XAxisSize;
    public int YAxisSize;


    public Vector3 cetroid = new Vector3(0, 0, 0);

    public Vector3 RoomCentroid()
    {
        if (CellInRoom == null || CellInRoom.Count == 0)
        {
            throw new System.Exception("Room has no cells to calculate centroid.");
        }

        float totalX = 0;
        float totalY = 0;

        foreach (var cell in CellInRoom)
        {
            totalX += cell.Coordinate.x;
            totalY += cell.Coordinate.y;
        }

        float centroidX = totalX / CellInRoom.Count;
        float centroidY = totalY / CellInRoom.Count;

        return new Vector3(centroidX, 1, centroidY);
    }

    public void MarkCorners()
    {
        if (CellInRoom == null || CellInRoom.Count == 0)
        {
            throw new System.Exception("Room has no cells to mark corners.");
        }

        // Znajdź minimalne i maksymalne współrzędne
        float minX = CellInRoom.Min(cell => cell.Coordinate.x);
        float maxX = CellInRoom.Max(cell => cell.Coordinate.x);
        float minY = CellInRoom.Min(cell => cell.Coordinate.y);
        float maxY = CellInRoom.Max(cell => cell.Coordinate.y);

        // Iteruj przez każdą komórkę i ustaw jako róg, jeśli spełnia kryteria
        foreach (var cell in CellInRoom)
        {
            if ((cell.Coordinate.x == minX && cell.Coordinate.y == minY) || // Lewy dolny róg
                (cell.Coordinate.x == minX && cell.Coordinate.y == maxY) || // Lewy górny róg
                (cell.Coordinate.x == maxX && cell.Coordinate.y == minY) || // Prawy dolny róg
                (cell.Coordinate.x == maxX && cell.Coordinate.y == maxY)) // Prawy górny róg
            {
                cell.SetIsRoomCorner(); // Wywołaj metodę w GridCellData
            }
        }
    }

    public void SpawnPlayer(GameObject playerPrefab, GameObject depositPrefab)
    {
        Vector3 spawnPosition = new Vector3();
        spawnPosition = RoomCentroid();

        var playerObj = SpawnObject(playerPrefab, spawnPosition, quaternion.identity);
        var deposit = SpawnObject(depositPrefab, spawnPosition + new Vector3(0.2f,-0.4f,0.2f), quaternion.identity);

        
        Camera cam = Camera.main;
        var camFollow = cam.AddComponent<LevelCameraFollow>();
        camFollow.player = playerObj;
        camFollow.offset = new Vector3(2, 2, 0);
        camFollow.followSpeed = 5f;
        camFollow.rotationSpeed = 10f;
        
        
    }

    public void SpawnPicakbleObject(List<GameObject> picakableObjects, int minAmount, int maxAmount)
    {
        if(RoomType == E_RoomType.SpawnRoom)
            return;
        int amount = Random.RandomRange(minAmount, maxAmount);
        if (amount == 0)
            return;
        
        
        for (int i = 0; i < amount; i++)
        {
            float x = Random.RandomRange(RoomCentroid().x - XAxisSize/2f + 0.5f, RoomCentroid().x + XAxisSize/2f -0.5f);
            float y = Random.RandomRange(RoomCentroid().z - YAxisSize/2f + 0.5f, RoomCentroid().z + YAxisSize/2f -0.5f);
            
            
            int randomIndex = Random.RandomRange(0, picakableObjects.Count);
            SpawnObject(picakableObjects[randomIndex], new Vector3(x,1,y), quaternion.identity);
        }
    }

    public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // Instantiating prefab
        GameObject newObject = Object.Instantiate(prefab, position, rotation);
        return newObject;
    }
}
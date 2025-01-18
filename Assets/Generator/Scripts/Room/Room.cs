using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
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
    public GameObject RoomParent;


    public Vector3 centroid = new Vector3(0, 0, 0);

    public void SetUpParent(GameObject roomParent, float size)
    {
        RoomParent = roomParent;
        roomParent.transform.localScale = new Vector3(size, size, size);
        roomParent.transform.localPosition = new Vector3(centroid.x, -1 , centroid.z);
    }
    
    public Vector3 RoomCentroid(float _cellSize)
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

        return new Vector3(centroidX * _cellSize, 0, centroidY * _cellSize);
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
        spawnPosition = centroid;

        GameObject playerObj = SpawnObject(playerPrefab, spawnPosition, quaternion.identity);
        GameObject deposit = SpawnObject(depositPrefab, spawnPosition + new Vector3(0.2f,-0.4f,0.2f), quaternion.identity);

        CinemachineFreeLook cinemachineFreeLook = Object.FindAnyObjectByType<CinemachineFreeLook>();
        cinemachineFreeLook.LookAt = playerObj.transform.Find("LookAt");
        cinemachineFreeLook.Follow = playerObj.transform.Find("Follow");
        
        /*
        Camera cam = Camera.main;
        var camFollow = cam.AddComponent<LevelCameraFollow>();
        camFollow.player = playerObj;
        camFollow.offset = new Vector3(2, 2, 0);
        camFollow.followSpeed = 5f;
        camFollow.rotationSpeed = 10f;*/


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
            float x = Random.RandomRange(centroid.x - XAxisSize/2f + 0.5f, centroid.x + XAxisSize/2f -0.5f);
            float y = Random.RandomRange(centroid.z - YAxisSize/2f + 0.5f, centroid.z + YAxisSize/2f -0.5f);
            
            
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

    public void GenerateRoomInside()
    {
        List<GameObject> allPrefabs = Resources.LoadAll<GameObject>("RoomInsidePrefab/" + XAxisSize + "x" + YAxisSize).ToList();
        
        if (allPrefabs.Count > 0)
        {
            int index =  Random.RandomRange(0, allPrefabs.Count);
            GameObject prefab = allPrefabs[index];
            GameObject spawnedPrefab = SpawnObject(prefab,Vector3.zero,Quaternion.identity);
            spawnedPrefab.transform.SetParent(RoomParent.transform);
            spawnedPrefab.transform.localPosition = new Vector3(0,0,0);
            spawnedPrefab.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("Room has no preapared prefab");
        }
    }

    public void DetectObjects(LayerMask inroomObjectLayer)
    {
        foreach (GridCellData cell in CellInRoom)
        {
            // Sprawdzanie, czy komórka ma odpowiedni typ (Pass lub SpawnPass)
            if (cell.GridCellType == E_GridCellType.Pass || cell.GridCellType == E_GridCellType.SpawnPass)
            {

                
                Vector3 boxCenter = cell.Position; // środek boxa
                Vector3 boxSize = new Vector3(cell.CellSize.x,cell.CellSize.y,cell.CellSize.x); // rozmiar boxa
                // Debugowanie środkowej pozycji boxa i jego rozmiaru
                
                GameObject obj = new GameObject();
                var gizmos = obj.AddComponent<BoxGizmos>();
                gizmos.SetUpGizmos(boxCenter, boxSize);
                
                // Przeszukiwanie obiektów w boxie
                Collider[] colliders = Physics.OverlapBox(boxCenter, boxSize, Quaternion.identity,inroomObjectLayer);

                if (colliders.Length == 0)
                {
                    Debug.Log("Brak obiektów w obszarze: " + boxCenter);
                }

                foreach (var collider in colliders)
                {
                    // Debugowanie, który obiekt został znaleziony
                    Debug.Log("Znaleziony obiekt: " + collider.gameObject.name);

                    // Rysowanie boxa, który wykrywa obiekty (pomaga wizualizować obszar poszukiwań)
                    
                    Object.Destroy(collider.GameObject());

                }
            }
            else
            {
                continue;
            }
        }
    }
}

public class BoxGizmos : MonoBehaviour
{
    private Vector3 center;
    private Vector3 size;

    public void SetUpGizmos(Vector3 center, Vector3 size)
    {
        this.center = center;
        this.size = size;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
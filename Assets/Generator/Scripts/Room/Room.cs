using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
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
    public GameObject RoomParent;
    public Dictionary<Room, bool> ContactRooms = new Dictionary<Room, bool>();


    public Vector3 centroid = new Vector3(0, 0, 0);
    
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
    
public void MarkCorners(List<Room> allRooms)
{
    /*if (CellInRoom == null || CellInRoom.Count == 0)
    {
        throw new System.Exception("Room has no cells to mark corners.");
    }

    // Zbiór komórek do analizy – zaczynamy od komórek bieżącego pokoju.
    HashSet<GridCellData> unionCells = new HashSet<GridCellData>(CellInRoom);
    
    // Dodajemy komórki z pokoi, które nachodzą na ten pokój (czyli mają przynajmniej jedną wspólną komórkę)
    foreach (var otherRoom in allRooms)
    {
        if (otherRoom == this)
            continue;
        
        // Jeśli istnieje przynajmniej jedna komórka wspólna, dodajemy wszystkie komórki tego pokoju.
        if (otherRoom.CellInRoom.Any(cell => CellInRoom.Contains(cell)))
        {
            foreach (var cell in otherRoom.CellInRoom)
            {
                unionCells.Add(cell);
            }
        }
    }

    // Wyznaczamy skrajne wartości wspólnego obszaru
    float minX = unionCells.Min(cell => cell.Coordinate.x);
    float maxX = unionCells.Max(cell => cell.Coordinate.x);
    float minY = unionCells.Min(cell => cell.Coordinate.y);
    float maxY = unionCells.Max(cell => cell.Coordinate.y);

    // W bieżącym pokoju oznaczamy jako narożniki te komórki, których współrzędne odpowiadają skrajnym wartościom
    foreach (var cell in CellInRoom)
    {
        if ((cell.Coordinate.x == minX && cell.Coordinate.y == minY) || // Lewy dolny róg
            (cell.Coordinate.x == minX && cell.Coordinate.y == maxY) || // Lewy górny róg
            (cell.Coordinate.x == maxX && cell.Coordinate.y == minY) || // Prawy dolny róg
            (cell.Coordinate.x == maxX && cell.Coordinate.y == maxY))   // Prawy górny róg
        {
            cell.SetIsRoomCorner();
        }
    }*/
}

public void MarkBorders(List<Room> allRooms)
{
    if (CellInRoom == null || CellInRoom.Count == 0)
    {
        throw new System.Exception("Room has no cells to mark borders.");
    }

    // Zbiór komórek do analizy – zaczynamy od komórek bieżącego pokoju.
    HashSet<GridCellData> unionCells = new HashSet<GridCellData>(CellInRoom);
    
    // Dodajemy komórki z pokoi, które nachodzą na ten pokój
    foreach (var otherRoom in allRooms)
    {
        if (otherRoom == this)
            continue;
        
        if (otherRoom.CellInRoom.Any(cell => CellInRoom.Contains(cell)))
        {
            foreach (var cell in otherRoom.CellInRoom)
            {
                unionCells.Add(cell);
            }
        }
    }

    // Wyznaczamy skrajne wartości wspólnego obszaru
    float minX = unionCells.Min(cell => cell.Coordinate.x);
    float maxX = unionCells.Max(cell => cell.Coordinate.x);
    float minY = unionCells.Min(cell => cell.Coordinate.y);
    float maxY = unionCells.Max(cell => cell.Coordinate.y);

    // Oznaczamy jako krawędź te komórki bieżącego pokoju, których współrzędne leżą na skraju wspólnego obszaru.
    foreach (var cell in CellInRoom)
    {
        if (cell.Coordinate.x == minX ||
            cell.Coordinate.x == maxX ||
            cell.Coordinate.y == minY ||
            cell.Coordinate.y == maxY)
        {
            cell.SetIsRoomBorder();
        }
    }
}

    
    public void SetUpParent(GameObject roomParent, float size)
    {
        RoomParent = roomParent;
        roomParent.transform.localScale = new Vector3(size, size, size);
        roomParent.transform.localPosition = new Vector3(centroid.x, -1, centroid.z);
    }
    

    /*public void SetUpParent(GameObject roomParent, float size)
    {
        RoomParent = roomParent;
        roomParent.transform.localScale = new Vector3(size, size, size);
        roomParent.transform.localPosition = new Vector3(centroid.x, -1, centroid.z);
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
        GameObject deposit = SpawnObject(depositPrefab, spawnPosition + new Vector3(0.2f, 0.32f, 0.2f),
            quaternion.identity);

        CinemachineFreeLook cinemachineFreeLook = Object.FindAnyObjectByType<CinemachineFreeLook>();
        cinemachineFreeLook.LookAt = playerObj.transform.Find("LookAt");
        cinemachineFreeLook.Follow = playerObj.transform.Find("Follow");

        /*
        Camera cam = Camera.main;
        var camFollow = cam.AddComponent<LevelCameraFollow>();
        camFollow.player = playerObj;
        camFollow.offset = new Vector3(2, 2, 0);
        camFollow.followSpeed = 5f;
        camFollow.rotationSpeed = 10f;#1#
    }

    public void SpawnPicakbleObject(List<GameObject> picakableObjects, int minAmount, int maxAmount)
    {
        if (RoomType == E_RoomType.SpawnRoom)
            return;
        int amount = Random.RandomRange(minAmount, maxAmount);
        if (amount == 0)
            return;


        for (int i = 0; i < amount; i++)
        {
            float x = Random.RandomRange(centroid.x - XAxisSize / 2f + 0.5f, centroid.x + XAxisSize / 2f - 0.5f);
            float y = Random.RandomRange(centroid.z - YAxisSize / 2f + 0.5f, centroid.z + YAxisSize / 2f - 0.5f);


            int randomIndex = Random.RandomRange(0, picakableObjects.Count);
            SpawnObject(picakableObjects[randomIndex], new Vector3(x, 1, y), quaternion.identity);
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
        List<GameObject> allPrefabs =
            Resources.LoadAll<GameObject>("RoomInsidePrefab/" + XAxisSize + "x" + YAxisSize).ToList();

        if (allPrefabs.Count > 0)
        {
            int index = Random.RandomRange(0, allPrefabs.Count);
            GameObject prefab = allPrefabs[index];
            GameObject spawnedPrefab = SpawnObject(prefab, Vector3.zero, Quaternion.identity);
            spawnedPrefab.transform.SetParent(RoomParent.transform);
            spawnedPrefab.transform.localPosition = new Vector3(0, 0, 0);
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
                Vector3 boxSize =
                    new Vector3(cell.CellSize.x / 2, cell.CellSize.y / 2, cell.CellSize.x / 2); // rozmiar boxa
                // Debugowanie środkowej pozycji boxa i jego rozmiaru

                GameObject obj = new GameObject();
                var gizmos = obj.AddComponent<BoxGizmos>();
                gizmos.SetUpGizmos(boxCenter, boxSize);

                // Przeszukiwanie obiektów w boxie
                Collider[] colliders = Physics.OverlapBox(boxCenter, boxSize, Quaternion.identity, inroomObjectLayer);

                foreach (var collider in colliders)
                {
                    Object.Destroy(collider.GameObject());
                }
            }
        }
    }

    public void SpawnDoors(GameObject doorPrefab, float doorSpawnOffset)
    {
        foreach (GridCellData cell in CellInRoom)
        {
            Vector3 rotation = Vector3.zero;
            Vector3 offset = Vector3.zero;
            switch (cell.AxisCell)
            {
                case var axis when axis == cell.UpN:
                    rotation = new Vector3(0, 90, 0);
                    offset = new Vector3(0, 0, doorSpawnOffset);
                    break;
                case var axis when axis == cell.DownN:
                    rotation = new Vector3(0, 270, 0);
                    offset = new Vector3(0, 0, -doorSpawnOffset);
                    break;
                case var axis when axis == cell.LeftN:
                    rotation = new Vector3(0, 0f, 0);
                    offset = new Vector3(-doorSpawnOffset, 0, 0);
                    break;
                case var axis when axis == cell.RightN:
                    rotation = new Vector3(0, 180, 0);
                    offset = new Vector3(doorSpawnOffset, 0, 0);
                    break;
                default:
                    Debug.LogWarning("Nieoczekiwany AxisCell");
                    break;
            }

            if (cell.GridCellType == E_GridCellType.Pass || cell.GridCellType == E_GridCellType.SpawnPass)
            {
                // Tworzenie drzwi
                GameObject obj = Object.Instantiate(doorPrefab, cell.Position, Quaternion.identity);

                // Ustawianie rotacji
                obj.transform.rotation = Quaternion.Euler(rotation);
                obj.transform.position += offset;
            }
        }
    }

    public void ChcekRoomContact(List<Room> AllRooms)
    {
        foreach (var room in AllRooms)
        {
            if (room != this)
                ContactRooms.Add(room, false);
        }

        foreach (var cell in CellInRoom)
        {
            var cellNeighbour = cell.ReturnNeighbour();
            foreach (var neigbour in cellNeighbour)
            {
                if (neigbour == null)
                    continue;

                if (neigbour.GetConnectedRoom() != null)
                {
                    ContactRooms[neigbour.GetConnectedRoom()] = true;
                }
            }
        }
    }

    public void SetRoomAssigned()
    {
        foreach (var cell in CellInRoom)
        {
            cell.SetupCellRoom(this);
        }
    }*/



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
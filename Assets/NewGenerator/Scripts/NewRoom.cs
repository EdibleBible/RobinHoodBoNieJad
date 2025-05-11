using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class NewRoom
{
    public E_RoomType RoomType;
    public int RoomID;
    public List<GridCellData> CellInRoom = new List<GridCellData>();
    public GeneratorRoomData RoomData;
    public int XAxisSize;
    public int YAxisSize;
    public Transform RoomParent;
    public bool IsSpawn;
    public GameObject SpawnedRoomObject;

    public Vector3 Centroid = new Vector3(0, 0, 0);

    public void CalculateRoomCentroid(float _cellSize)
    {
        float totalX = 0f;
        float totalY = 0f;

        foreach (var cell in CellInRoom)
        {
            float centerX = cell.Coordinate.x * _cellSize;
            float centerY = cell.Coordinate.y * _cellSize;

            totalX += centerX;
            totalY += centerY;
        }

        Centroid = new Vector3(totalX / CellInRoom.Count(), 0, totalY / CellInRoom.Count());
    }

    public Vector3 GetRoomCentroid()
    {
        return Centroid;
    }

    public void SpawnPrefabs(int wallPassLayerInt, int wallPassSpawnLayerInt)
    {
        GameObject[] prefabs;
        if (!IsSpawn)
            prefabs = Resources.LoadAll<GameObject>($"RoomsPrefab/{YAxisSize}x{XAxisSize}");
        else
            prefabs = Resources.LoadAll<GameObject>($"RoomsSpawnPrefab/{YAxisSize}x{XAxisSize}");
        
        if (prefabs.Length > 0)
        {
            // Wylosuj jeden
            int randomIndex = UnityEngine.Random.Range(0, prefabs.Length);
            GameObject selectedPrefab = prefabs[randomIndex];

            // Instantiate go np. w (0, 0, 0) z domyślną rotacją
             var obj = GameObject.Instantiate(selectedPrefab, GetRoomCentroid(), Quaternion.identity, RoomParent);
            SpawnedRoomObject = obj;
            
            RoomData = obj.GetComponent<GeneratorRoomData>();
            Transform[] selectedFloors =
                RoomData.AllFloors.Where(x => x.gameObject.layer == wallPassLayerInt || x.gameObject.layer == wallPassSpawnLayerInt).ToArray();

            foreach (var cell in CellInRoom)
            {
                if (selectedFloors.Any(x => x.position.x == cell.Position.x && x.position.z == cell.Position.z))
                {
                    cell.GridCellType = E_GridCellType.Pass;
                }
            }
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

    public List<GridCellData> GetPassGridCell()
    {
        List<GridCellData> passCells = new List<GridCellData>();
        foreach (var cell in CellInRoom)
        {
            if (cell.GridCellType == E_GridCellType.Pass)
            {
                passCells.Add(cell);
            }
        }

        return passCells;
    }

    public void SetInteractableObjectNullParent()
    {
        if (SpawnedRoomObject == null)
            return;

        foreach (Transform child in SpawnedRoomObject.transform)
        {
            if (child.TryGetComponent<IInteractable>(out var interactable))
            {
                child.SetParent(null);
            }
        }
    }
}
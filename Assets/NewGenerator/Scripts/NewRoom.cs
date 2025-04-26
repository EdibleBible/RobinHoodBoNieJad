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
                if (selectedFloors.Any(x => x.position.x - 1 == cell.Position.x && x.position.z - 1 == cell.Position.z))
                {
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
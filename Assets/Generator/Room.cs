using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        return new Vector3(centroidX,1, centroidY);
    }
}

using System;
using UnityEngine;

[Serializable]
public class GridCellData
{
    public E_GridCellType GridCellType;
    public Vector2Int Coordinate;
    public Vector3 Position;

    public void SetCoordinate(Vector2Int coordinate)
    {
        Coordinate = coordinate;
    }

    public void SetCoordinate(int x, int y)
    {
        SetCoordinate(new Vector2Int(x, y));
    }

    public void SetPosition(Vector3 postion)
    {
        Position = postion;
    }

    public void SetPosition(float x, float y, float z)
    {
        SetPosition(new Vector3(x, y, z));
    }
}

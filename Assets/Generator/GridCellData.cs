using CustomGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GridCellData
{
    public Grid<GridCellData> GridHolder;
    public E_GridCellType GridCellType;
    public Vector2Int Coordinate;
    public Vector3 Position;
    public List<GridCellData> neigbourCell = new List<GridCellData>();
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

    public void SetGridParent(Grid<GridCellData> grid)
    {
        GridHolder = grid;

    }
} 


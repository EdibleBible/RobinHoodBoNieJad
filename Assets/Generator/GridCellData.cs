using CustomGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class GridCellData
{
    public Grid<GridCellData> GridHolder;
    public E_GridCellType GridCellType;
    public Vector2Int Coordinate;
    public Vector3 Position;
    public GridCellData BottomN;
    public GridCellData UpN;
    public GridCellData LeftN;
    public GridCellData RightN;
    public GridCellData AxisCell;
    public Vector2 CellSize;
    public bool IsRoomCorner = false;

    public void SetAxisCell(GridCellData axisCell)
    {
        AxisCell = axisCell;
    }
    public void SetIsRoomCorner()
    {
        IsRoomCorner = true;
    }
    public void SetCellSize(Vector2 size)
    {
        CellSize = size;
    }
    public void SetCellSize(float cellScale)
    {
        Vector2 scale = new Vector2(cellScale, cellScale);
        SetCellSize(scale);
    }
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
        var neigbourCell = grid.GetNeighbourList(this, false);

        BottomN = null;
        UpN = null;
        LeftN = null;
        RightN = null;

        foreach (var neighbor in neigbourCell)
        {
            Vector2Int delta = neighbor.Coordinate - this.Coordinate;

            if (delta == new Vector2Int(0, 1))
            {
                UpN = neighbor;
            }
            else if (delta == new Vector2Int(0, -1))
            {
                BottomN = neighbor;
            }
            else if (delta == new Vector2Int(-1, 0))
            {
                LeftN = neighbor;
            }
            else if (delta == new Vector2Int(1, 0))
            {
                RightN = neighbor;
            }
        }
    }

    public List<Matrix4x4> AllCellMatrix4x4(Vector2 segmentSize)
    {
        List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();
        int wallCount = 0;
        float scale = 0f;

        //x
        wallCount = math.max(0, (int)(CellSize.x / segmentSize.x));
        scale = CellSize.x / wallCount / segmentSize.x;



        //x up
        if (IsRoomCorner)
        {
            if (GridCellType == E_GridCellType.Pass || GridCellType == E_GridCellType.SecretPass || GridCellType == E_GridCellType.SpawnPass)
            {
                if(UpN != AxisCell && (UpN.GridCellType == E_GridCellType.Empty || UpN.GridCellType == E_GridCellType.Hallway))
                {
                    for (int i = 0; i < wallCount; i++)
                    {
                        Vector3 t = Position + new Vector3(/*(-CellSize.x / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale)*/(i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0, CellSize.y / 2f);
                        Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                        Vector3 s = new Vector3(1f, 1f, scale);

                        Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                        matrix4X4s.Add(newMatrix);
                    }
                }
            }
            else
            {
                if ((UpN.GridCellType == E_GridCellType.Empty || UpN.GridCellType == E_GridCellType.Hallway) && GridCellType != E_GridCellType.Pass && GridCellType != E_GridCellType.SecretPass && GridCellType != E_GridCellType.SpawnPass)
                {
                    for (int i = 0; i < wallCount; i++)
                    {
                        Vector3 t = Position + new Vector3(/*(-CellSize.x / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale)*/(i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0, CellSize.y / 2f);
                        Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                        Vector3 s = new Vector3(1f, 1f, scale);

                        Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                        matrix4X4s.Add(newMatrix);
                    }
                }
            }
        }
        else
        {
            if ((UpN.GridCellType == E_GridCellType.Empty || UpN.GridCellType == E_GridCellType.Hallway) && GridCellType != E_GridCellType.Pass && GridCellType != E_GridCellType.SecretPass && GridCellType != E_GridCellType.SpawnPass)
            {
                for (int i = 0; i < wallCount; i++)
                {
                    Vector3 t = Position + new Vector3(/*(-CellSize.x / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale)*/(i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0, CellSize.y / 2f);
                    Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 s = new Vector3(1f, 1f, scale);

                    Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                    matrix4X4s.Add(newMatrix);
                }
            }
        }



        //x down
        if (IsRoomCorner)
        {
            if (GridCellType == E_GridCellType.Pass || GridCellType == E_GridCellType.SecretPass || GridCellType == E_GridCellType.SpawnPass)
            {
                if (BottomN != AxisCell && (BottomN.GridCellType == E_GridCellType.Empty || BottomN.GridCellType == E_GridCellType.Hallway))
                {
                    for (int i = 0; i < wallCount; i++)
                    {
                        Vector3 t = Position + new Vector3(/*(-CellSize.x / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale)*/(i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0, -CellSize.y / 2f);
                        Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                        Vector3 s = new Vector3(1f, 1f, scale);

                        Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                        matrix4X4s.Add(newMatrix);
                    }
                }
            }
            else
            {
                if ((BottomN.GridCellType == E_GridCellType.Empty || BottomN.GridCellType == E_GridCellType.Hallway) && GridCellType != E_GridCellType.Pass && GridCellType != E_GridCellType.SecretPass && GridCellType != E_GridCellType.SpawnPass)
                {
                    for (int i = 0; i < wallCount; i++)
                    {
                        Vector3 t = Position + new Vector3(/*(-CellSize.x / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale)*/(i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0, -CellSize.y / 2f);
                        Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                        Vector3 s = new Vector3(1f, 1f, scale);

                        Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                        matrix4X4s.Add(newMatrix);
                    }
                }
            }
        }
        else
        {
            if ((BottomN.GridCellType == E_GridCellType.Empty || BottomN.GridCellType == E_GridCellType.Hallway) && GridCellType != E_GridCellType.Pass && GridCellType != E_GridCellType.SecretPass && GridCellType != E_GridCellType.SpawnPass)
            {
                for (int i = 0; i < wallCount; i++)
                {
                    Vector3 t = Position + new Vector3(/*(-CellSize.x / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale)*/(i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0, -CellSize.y / 2f);
                    Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 s = new Vector3(1f, 1f, scale);

                    Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                    matrix4X4s.Add(newMatrix);
                }
            }
        }



        // y
        wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
        scale = CellSize.y / wallCount / segmentSize.y;



        //y right
        if (IsRoomCorner)
        {
            if (GridCellType == E_GridCellType.Pass || GridCellType == E_GridCellType.SecretPass || GridCellType == E_GridCellType.SpawnPass)
            {
                if (RightN != AxisCell && (RightN.GridCellType == E_GridCellType.Empty || RightN.GridCellType == E_GridCellType.Hallway))
                {
                    for (int i = 0; i < wallCount; i++)
                    {
                        Vector3 t = Position + new Vector3(CellSize.x / 2f, 0, (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
                        Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                        Vector3 s = new Vector3(1f, 1f, scale);

                        Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                        matrix4X4s.Add(newMatrix);
                    }
                }
            }
            else
            {
                if ((RightN.GridCellType == E_GridCellType.Empty || RightN.GridCellType == E_GridCellType.Hallway)
                    && GridCellType != E_GridCellType.Pass
                    && GridCellType != E_GridCellType.SecretPass
                    && GridCellType != E_GridCellType.SpawnPass)
                {
                    for (int i = 0; i < wallCount; i++)
                    {
                        Vector3 t = Position + new Vector3(CellSize.x / 2f, 0, (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
                        Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                        Vector3 s = new Vector3(1f, 1f, scale);

                        Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                        matrix4X4s.Add(newMatrix);
                    }
                }
            }
        }
        else
        {
            if ((RightN.GridCellType == E_GridCellType.Empty || RightN.GridCellType == E_GridCellType.Hallway)
                && GridCellType != E_GridCellType.Pass
                && GridCellType != E_GridCellType.SecretPass
                && GridCellType != E_GridCellType.SpawnPass)
            {
                for (int i = 0; i < wallCount; i++)
                {
                    Vector3 t = Position + new Vector3(CellSize.x / 2f, 0, (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
                    Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                    Vector3 s = new Vector3(1f, 1f, scale);

                    Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                    matrix4X4s.Add(newMatrix);
                }
            }

        }



        //y left
        if (IsRoomCorner)
        {
            if (GridCellType == E_GridCellType.Pass || GridCellType == E_GridCellType.SecretPass || GridCellType == E_GridCellType.SpawnPass)
            {
                if (LeftN != AxisCell && (LeftN.GridCellType == E_GridCellType.Empty || LeftN.GridCellType == E_GridCellType.Hallway))
                {
                    for (int i = 0; i < wallCount; i++)
                    {
                        Vector3 t = Position + new Vector3(-CellSize.x / 2f, 0, (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
                        Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                        Vector3 s = new Vector3(1f, 1f, scale);

                        Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                        matrix4X4s.Add(newMatrix);

                    }
                }
            }
            else
            {
                if ((LeftN.GridCellType == E_GridCellType.Empty || LeftN.GridCellType == E_GridCellType.Hallway)
                    && GridCellType != E_GridCellType.Pass
                    && GridCellType != E_GridCellType.SecretPass
                    && GridCellType != E_GridCellType.SpawnPass)
                {
                    for (int i = 0; i < wallCount; i++)
                    {
                        Vector3 t = Position + new Vector3(-CellSize.x / 2f, 0, (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
                        Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                        Vector3 s = new Vector3(1f, 1f, scale);

                        Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                        matrix4X4s.Add(newMatrix);

                    }
                }
            }
        }
        else
        {
            if ((LeftN.GridCellType == E_GridCellType.Empty || LeftN.GridCellType == E_GridCellType.Hallway)
                && GridCellType != E_GridCellType.Pass
                && GridCellType != E_GridCellType.SecretPass
                && GridCellType != E_GridCellType.SpawnPass)
            {
                for (int i = 0; i < wallCount; i++)
                {
                    Vector3 t = Position + new Vector3(-CellSize.x / 2f, 0, (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
                    Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                    Vector3 s = new Vector3(1f, 1f, scale);

                    Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                    matrix4X4s.Add(newMatrix);

                }
            }
        }
        return matrix4X4s;
    }


}


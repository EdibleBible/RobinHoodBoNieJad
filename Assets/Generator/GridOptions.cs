using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, ExecuteAlways]
public struct GridOptions
{
    [Header("Grid Material")]
    public Material EmptyCellMaterial;
    public Material RoomCellMaterial;
    public Material PassCellMaterial;
    public Material HallwayMaterial;
    public Material SpawnRoomMaterial;
    public Material SpawnPassMaterial;
    public Material SecretHallwayMaterial;
    public Material SecretPassMaterial;

    [Header("Grid Parameters")]
    public Vector2Int MaxAxisSize;
    public Vector2Int MinAxisSize;
    public float cellScale;

    public Vector2Int RandomizeGridSize(System.Random random)
    {
        int randomX = random.Next(MinAxisSize.x,MaxAxisSize.x);
        int randomY = random.Next(MinAxisSize.y,MaxAxisSize.y);
        return new Vector2Int(randomX, randomY);
    }
}



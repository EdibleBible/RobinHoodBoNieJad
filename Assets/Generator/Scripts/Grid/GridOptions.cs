using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[Serializable, ExecuteAlways]
public struct GridOptions
{
    [Header("Grid Material")]
    public SerializedDictionary<E_GridCellType, Material> materials;
    [Header("Grid Parameters")]
    public Vector2Int MaxAxisSize;
    public Vector2Int MinAxisSize;
    public float cellScale;

    public Vector2Int RandomizeGridSize(uint seed)
    {
        var random = new Unity.Mathematics.Random(seed);

        int randomX = random.NextInt(MinAxisSize.x,MaxAxisSize.x);
        int randomY = random.NextInt(MinAxisSize.y,MaxAxisSize.y);
        return new Vector2Int(randomX, randomY);
    }
}



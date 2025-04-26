using System;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[Serializable]
public struct GridParameters
{
    public bool IsRandomized;
    public bool RandomizeSeed;

    public Vector2Int GridSize;
    public Vector2Int MaxGridSize;
    public Vector2Int MinGridSize;

    public float CellSize;
    public AYellowpaper.SerializedCollections.SerializedDictionary<E_GridCellType, Material> materials;

    public void RandomizeGridSize(uint seed)
    {
        Random random = new Random(seed);

        if (IsRandomized)
        {
            int x = random.NextInt(MinGridSize.x, MaxGridSize.x);
            int y = random.NextInt(MinGridSize.y, MaxGridSize.y);

            GridSize = new Vector2Int(x, y);
        }
    }
}
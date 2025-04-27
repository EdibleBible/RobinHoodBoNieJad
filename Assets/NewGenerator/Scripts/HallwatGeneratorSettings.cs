using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public struct MeshGeneratorSettings
{
    public List<GridCellData> HallwayCell;
    public Dictionary<Mesh, List<Matrix4x4>> AllMatrix;
    
    public Vector2 WallSegmentSize;
    public Vector2 FloorSegmentSize;

    public List<Mesh> FloorMeshes;
    public List<Mesh> WallMeshes;
    
    public Material WallMaterial;
    public Material FloorMaterial;
    
    public Transform MeshesParent;
    
    public LayerMask WallLayerMask;
    public LayerMask FloorLayerMask;

    public List<GridCellData> GetShuffledHallwayCells()
    {
        List<GridCellData> shuffledList = new List<GridCellData>(HallwayCell);

        for (int i = shuffledList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffledList[i], shuffledList[randomIndex]) = (shuffledList[randomIndex], shuffledList[i]);
        }

        return shuffledList;
    }

    
}
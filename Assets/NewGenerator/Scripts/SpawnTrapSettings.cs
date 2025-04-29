using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SpawnTrapSettings
{
    public List<GameObject> TrapsPrefabs;
    public int MaxTrapsCount;
    [UnityEngine.Range(0,100)]public float ChanceToSpawn;
}
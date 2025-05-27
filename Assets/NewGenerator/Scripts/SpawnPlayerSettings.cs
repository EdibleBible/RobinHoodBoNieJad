using System;
using UnityEngine;

[Serializable]
public struct SpawnPlayerSettings
{
    public GameObject PlayerPrefab;
    public GameObject CanvasPrefab;
    public GameObject ExitPrefab;
    
    public Transform PlayerTransform;
    
    public Vector3 SpawnOffset;
}
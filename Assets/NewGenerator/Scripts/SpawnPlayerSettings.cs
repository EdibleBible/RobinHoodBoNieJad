using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public struct SpawnPlayerSettings
{
    public GameObject ExitPrefab;
    public GameObject PlayerPrefab;
    public GameObject CanvasPrefab;
    
    public Transform PlayerTransform;
    
    public Vector3 SpawnOffset;
}
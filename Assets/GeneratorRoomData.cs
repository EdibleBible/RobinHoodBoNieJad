using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GeneratorRoomData : MonoBehaviour
{
    public List<Transform> AllFloors = new List<Transform>();
    public List<DoorController> AllDoors = new List<DoorController>();
    public Transform SpawnPosition;
    public bool IsSpawn = false;

    [Tooltip("Warstwy, które mają być traktowane jako podłogi")]
    public LayerMask floorLayers;

    private void OnEnable()
    {
        var obj = FindObjectsByType<ItemBase>(FindObjectsSortMode.None);
        foreach (var singleObj in obj)
        {
            singleObj.transform.SetParent(null);
        }

        foreach (var door in AllDoors)
        {
            door.transform.SetParent(null);
        }
    }
}
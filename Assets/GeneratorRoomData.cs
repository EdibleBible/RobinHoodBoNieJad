using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GeneratorRoomData : MonoBehaviour
{
    public List<Transform> AllFloors = new List<Transform>();
    public List<DoorController> AllDoors = new List<DoorController>();
     public Transform SpawnPosition;
    
    [Tooltip("Warstwy, które mają być traktowane jako podłogi")]
    public LayerMask floorLayers;
}


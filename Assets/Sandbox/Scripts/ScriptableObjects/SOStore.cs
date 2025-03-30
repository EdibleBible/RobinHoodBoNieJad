using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOStore", menuName = "Scriptable Objects/SOStore")]
public class SOStore : ScriptableObject
{
    public List<ItemData> storeItems = new();
    public List<StoreEntry> storeEntries = new();
}

[Serializable] public class StoreEntry
{
    public int keyVisit;
    public int tradeDuration;
    public GameObject itemPrefab;
}
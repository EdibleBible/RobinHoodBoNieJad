using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOStore", menuName = "Scriptable Objects/SOStore")]
public class SOStore : ScriptableObject
{
    public List<ItemModifier> storeItems = new();
}

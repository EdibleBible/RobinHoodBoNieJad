using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOInventory", menuName = "Scriptable Objects/SOInventory")]
public class SOInventory : ScriptableObject
{
    public List<ItemBase> itemList = new();
    public int playerScore;
}

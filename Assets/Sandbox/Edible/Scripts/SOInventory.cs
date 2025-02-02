using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SOInventory", menuName = "Scriptable Objects/SOInventory")]
public class SOInventory : ScriptableObject
{
    [HideInInspector] public int CurrInventoryLoad;

    
    public SerializedDictionary<ItemType, float> ItemsValue = new SerializedDictionary<ItemType, float>();
    public List<ItemBase> ItemsInInventory = new List<ItemBase>();
    public float CurrInvenoryScore;
    public int InventorySize;

    public void ClearInventory()
    {
        CurrInvenoryScore = 0;
        ItemsInInventory.Clear();
        CurrInventoryLoad = 0;
    }

    public bool AddItemToInventory(ItemBase item)
    {
        int tempoleryInventoryLoad = CurrInventoryLoad + item.ItemSize;

        if (tempoleryInventoryLoad > InventorySize)
        {
            Debug.LogWarning("Inventory full");
            return false;
        }
        
        ItemsInInventory.Add(item);
        CurrInvenoryScore = CurrInventoryLoad;
        CurrInventoryLoad = tempoleryInventoryLoad;
        return true;
    }

    public bool RemoveItemFromInventory(ItemBase item)
    {
        if (!ItemsInInventory.Contains(item))
        {
            Debug.Log("Item is not in inventory");
            return false;
        }
        
        ItemsInInventory.Remove(item);
        CurrInvenoryScore = CalculateInvenoryScore();
        CurrInvenoryScore -= item.ItemSize;
        return true;
    }

    public float CalculateInvenoryScore()
    {
        float score = 0;
        foreach (var item in ItemsInInventory)
        {
            score += ItemsValue[item.ItemType];
        }

        return score;
    }
}
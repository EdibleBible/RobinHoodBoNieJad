using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SOInventory", menuName = "Scriptable Objects/SOInventory")]
public class SOInventory : ScriptableObject
{
    [HideInInspector] public int CurrInventoryLoad;
    
    public List<ItemData> ItemsInInventory = new List<ItemData>();
    public float CurrInvenoryScore;
    public int InventorySize;
    
    public void ClearInventory()
    {
        CurrInvenoryScore = 0;
        ItemsInInventory.Clear();
        CurrInventoryLoad = 0;
    }

    public void SetUpInventory()
    {
        CurrInvenoryScore = CalculateInvenoryScore();
    }

    public bool AddItemToInventory(ItemData item)
    {
        int tempoleryInventoryLoad = CurrInventoryLoad + item.ItemSize;

        if (tempoleryInventoryLoad > InventorySize)
        {
            Debug.Log("Inventory full");
            return false;
        }

        ItemsInInventory.Add(item);
        CurrInvenoryScore = CurrInventoryLoad;
        CurrInventoryLoad = tempoleryInventoryLoad;
        return true;
    }

    public bool RemoveItemFromInventory(ItemData item)
    {
        if (!ItemsInInventory.Contains(item))
        {
            Debug.Log("Item is not in inventory");
            return false;
        }

        ItemsInInventory.Remove(item);
        CurrInvenoryScore = CalculateInvenoryScore();
        CurrInventoryLoad -= item.ItemSize;
        Debug.Log(CurrInventoryLoad);
        return true;
    }

    public float CalculateInvenoryScore()
    {
        float score = 0;
        foreach (var item in ItemsInInventory)
        {
            if (item != null)
                score += item.ItemValue;
            else
            {
                Debug.Log("Item is not in inventory");
                continue;
            }
        }

        return score;
    }

    public int CalculateInventoryLoad()
    {
        int load = 0;
        foreach (var item in ItemsInInventory)
        {
            if (item != null)
                load += item.ItemSize;
            else
            {
                Debug.LogWarning("Item is not in inventory");
                continue;
            }
        }
        
        return load;
    }
}
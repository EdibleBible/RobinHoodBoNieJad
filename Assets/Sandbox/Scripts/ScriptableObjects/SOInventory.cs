using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Script.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SOInventory", menuName = "Scriptable Objects/SOInventory")]
public class SOInventory : ScriptableObject
{
    public SOPlayerStatsController PlayerStatsController;

    public int CurrInventoryLoad;

    public List<ItemData> ItemsInInventory = new List<ItemData>();
    public List<ItemData> InventoryLobby = new List<ItemData>();
    public float CurrInvenoryScore;
    public float ScoreBackup;
    public int BaseInventorySize;
    public int CurrInventorySize;
    public List<ItemType> CollectedItemTypes;
    public int CollectedGoblets;
    public int CollectedVases;
    public int CollectedBooks;
    
    public void LoadInventory(List<ItemDataSave> saveDataItemsInInventory, List<ItemDataSave> saveDataInventoryLobby, float saveDataCurrentInvenoryScore, float saveDataScoreBackpack, int saveDataBaseInventorySize, int saveDataCurrInventorySize, List<int> saveDataCollectedItemsType, int saveDataCollectedGoblets, int saveDataCollectedVases, int saveDataCollectedBooks)
    {
        ItemsInInventory.Clear();
        foreach (var item in saveDataItemsInInventory)
        {
            ItemBase selectedItem = FindFirstItem((ItemType)item.ItemType, item.ItemName);
            
            if (selectedItem == null)
            {
                Debug.LogError("no item found");
                continue;
            }
            
            ItemsInInventory.Add(selectedItem.ItemData);
        }
        
        InventoryLobby.Clear();
        foreach (var item in saveDataInventoryLobby)
        {
            ItemBase selectedItem = FindFirstItem((ItemType)item.ItemType, item.ItemName);
            
            if (selectedItem == null)
            {
                Debug.LogError("no item found");
                continue;
            }
            
            InventoryLobby.Add(selectedItem.ItemData);
        }
        
        CurrInvenoryScore = saveDataCurrentInvenoryScore;
        ScoreBackup = saveDataScoreBackpack;
        BaseInventorySize = saveDataBaseInventorySize;
        CurrInventorySize = saveDataCurrInventorySize;

        CollectedItemTypes.Clear();
        foreach (var saveItemType in saveDataCollectedItemsType)
        {
            CollectedItemTypes.Add((ItemType)saveItemType);
        }
        
        CollectedGoblets = saveDataCollectedGoblets;
        CollectedVases = saveDataCollectedVases;
        CollectedBooks = saveDataCollectedBooks;
    }
    
    public ItemBase FindFirstItem(ItemType type, string name)
    {
        GameObject[] allPickables = Resources.LoadAll<GameObject>("Pickable");

        foreach (GameObject prefab in allPickables)
        {
            ItemBase item = prefab.GetComponent<ItemBase>();
            if (item != null && item.ItemData.ItemType == type && item.ItemData.ItemName == name)
            {
                return item;
            }
        }

        return null;
    }
    public ItemBase FindRandomItem()
    {
        GameObject[] allPickables = Resources.LoadAll<GameObject>("Pickable");
        List<ItemBase> validItems = new List<ItemBase>();

        foreach (GameObject prefab in allPickables)
        {
            ItemBase item = prefab.GetComponent<ItemBase>();
            if (item != null)
            {
                validItems.Add(item);
            }
        }

        if (validItems.Count > 0)
        {
            int randomIndex = Random.Range(0, validItems.Count);
            return validItems[randomIndex];
        }

        return null;
    }


    
    public void CalculateItemsSlotsCount()
    {
        CurrInventorySize =
            (BaseInventorySize +
             (int)Math.Floor(PlayerStatsController.GetSOPlayerStats(E_ModifiersType.Inventory).Additive)) *
             (int)Math.Floor(PlayerStatsController.GetSOPlayerStats(E_ModifiersType.Inventory).Multiplicative);
    }

    public void ClearInventory()
    {
        CurrInvenoryScore = 0;
        ItemsInInventory.Clear();
        CurrInventoryLoad = 0;
    }

    public void SetUpInventory()
    {
        CurrInvenoryScore = CalculateInvenoryScore();
        CurrInventoryLoad = 0;
        
        foreach (var item in ItemsInInventory)
        {
            CurrInventoryLoad += item.ItemSize;
        }
    }

    public bool AddItemToInventory(ItemData item)
    {
        int tempoleryInventoryLoad = CurrInventoryLoad;
        tempoleryInventoryLoad += item.ItemSize;

        if (tempoleryInventoryLoad > CurrInventorySize)
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
        Debug.Log($"RemoveItemFromInventory: {item.ReturnData()}");
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

    public void CollectGoblet(int id)
    {
        CollectedGoblets |= 1 << (id - 1);
    }

    public void CollectVase(int id)
    {
        CollectedVases |= 1 << (id - 1);
    }

    public void CollectBook(int id)
    {
        CollectedBooks |= 1 << (id - 1);
    }

    public bool IsCollectedGoblet(int id)
    {
        return (CollectedGoblets & (1 << (id - 1))) != 0;
    }

    public bool IsCollectedVase(int id)
    {
        return (CollectedVases & (1 << (id - 1))) != 0;
    }

    public bool IsCollectedBook(int id)
    {
        return (CollectedBooks & (1 << (id - 1))) != 0;
    }

    public void UncollectGoblet(int id)
    {
        CollectedGoblets &= ~(1 << (id - 1));
    }

    public void UncollectVase(int id)
    {
        CollectedVases &= ~(1 << (id - 1));
    }

    public void UncollectBook(int id)
    {
        CollectedBooks &= ~(1 << (id - 1));
    }
    public void ResetCollections()
    {
        CollectedGoblets = 0;
        CollectedVases = 0;
        CollectedBooks = 0;
    }
}
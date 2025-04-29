using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Script.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SOInventory", menuName = "Scriptable Objects/SOInventory")]
public class SOInventory : ScriptableObject
{
    public SOPlayerStatsController PlayerStatsController;

    [HideInInspector] public int CurrInventoryLoad;

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
        InventoryLobby.Clear();
        CurrInventoryLoad = 0;
    }

    public void SetUpInventory()
    {
        CurrInvenoryScore = CalculateInvenoryScore();
    }

    public bool AddItemToInventory(ItemData item)
    {
        int tempoleryInventoryLoad = CurrInventoryLoad + item.ItemSize;

        if (tempoleryInventoryLoad > CurrInventorySize)
        {
            Debug.Log("Inventory full");
            return false;
        }

        InventoryLobby.Add(item);
        CurrInvenoryScore = CurrInventoryLoad;
        CurrInventoryLoad = tempoleryInventoryLoad;
        return true;
    }

    public bool RemoveItemFromInventory(ItemData item)
    {
        if (!InventoryLobby.Contains(item))
        {
            Debug.Log("Item is not in inventory");
            return false;
        }

        InventoryLobby.Remove(item);
        CurrInvenoryScore = CalculateInvenoryScore();
        CurrInventoryLoad -= item.ItemSize;
        Debug.Log(CurrInventoryLoad);
        return true;
    }

    public float CalculateInvenoryScore()
    {
        float score = 0;
        foreach (var item in InventoryLobby)
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
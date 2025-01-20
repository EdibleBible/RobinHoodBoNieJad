using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemBase> itemList = new();
    public List<ItemBase> slotList = new();
    public int inventorySize = 3;
    public SOStats stats;
    public int finalPrize;
    public bool playerCollides;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        inventorySize = stats.inventorySize;
        ItemBase.OnItemAdded += AddItemToInventory;
    }

    private void OnDisable()
    {
        ItemBase.OnItemAdded -= AddItemToInventory;
    }

    // Triggered when an item broadcasts its data
    private void AddItemToInventory(ItemBase itemData)
    {
        if (SizeCheck(itemData.itemSize))
        {
            itemData.gameObject.SetActive(false);
            itemData.gameObject.transform.SetParent(this.transform);
            for (int i = 0; i < itemData.itemSize; i++)
            {
                slotList.Add(itemData);
            }
            itemList.Add(itemData);
        }
    }

    private bool SizeCheck(int itemSize)
    {
        if (slotList.Count() - 1 + itemSize < inventorySize)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCollides = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCollides = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerCollides)
        {
            int localPrize;
            foreach (ItemBase item in itemList)
            {
                localPrize = RandomizePrize(item.itemTypeValues[((int)item.itemType)]);
                finalPrize += localPrize;
                stats.scoreLevel += localPrize;
            }
            itemList.Clear();
            slotList.Clear();
        }
    }

    private int RandomizePrize(int midpoint)
    {
        return Random.Range((int)(midpoint * 0.9), (int)(midpoint * 1.1));
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemBase> itemList = new();
    public List<ItemBase> slotList = new();
    public int inventorySize = 3;
    public SOStats stats;
    public int finalPrize;

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
        if (slotList.Count() + itemSize < inventorySize)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (ItemBase item in itemList)
            {
                finalPrize += RandomizePrize(item.itemTypeValues[((int)item.itemType)]);
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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInventoryChoose : MonoBehaviour
{
    public GameObject inventoryEntry;
    public SOInventory inventory;
    public GameObject inventoryParent;
    List<GameObject> itemsSlot = new List<GameObject>();
    
    public Sprite frameInactive;
    public Sprite frameActive;

    public void OnEnable()
    {
        var holderInventoryInLobbby = inventory.InventoryLobby;
        holderInventoryInLobbby.AddRange(inventory.ItemsInInventory);
        inventory.ItemsInInventory.Clear();
        inventory.InventoryLobby = holderInventoryInLobbby;

        ReloadItemDisplay();
    }

    public void ReloadItemDisplay()
    {
        foreach (var slot in itemsSlot)
        {
            DestroyImmediate(slot);
        }

        itemsSlot.Clear();

        foreach (var item in inventory.InventoryLobby)
        {
            if (item.ItemType == ItemType.CollectibleVase || item.ItemType == ItemType.CollectibleGoblet ||
                item.ItemType == ItemType.CollectibleBook)
            {
                continue;
            }

            GameObject itemFrame = Instantiate(inventoryEntry, inventoryParent.transform);

            itemsSlot.Add(itemFrame);

            var entry = itemFrame.GetComponent<LobbyInventoryChooseEntry>();
            entry.parent = this;
            entry.itemIcon.sprite = item.ItemIcon;
            entry.data = item;
        }
    }

    public void SelectFrame(bool isSelected, LobbyInventoryChooseEntry entry, ItemData itemData)
    {
        entry.isSelected = !isSelected;
        itemData.ProceedToDungeon = !isSelected;
        if (!isSelected)
        {
            entry.frame.sprite = frameActive;
        }
        else
        {
            entry.frame.sprite = frameInactive;
        }
        
        if (entry.isSelected)
        {
            inventory.ItemsInInventory.Add(itemData);
            inventory.InventoryLobby.Remove(itemData);
        }
        else
        {
            inventory.ItemsInInventory.Remove(itemData);
            inventory.InventoryLobby.Add(itemData);
        }
    }
}

/*public GameObject inventoryEntry;
public SOInventory inventory;
public GameObject inventoryParent;

public List<GameObject> inventoryItems = new();

public Sprite frameInactive;
public Sprite frameActive;

private void OnEnable()
{
    Reload();
}

private void OnDisable()
{
    foreach (var item in inventoryItems)
    {
        Destroy(item.gameObject);
    }

    inventoryItems.Clear();
}

public void Reload()
{
    int i = 0;

    var holderList = inventory.InventoryLobby;
    holderList.AddRange(inventory.ItemsInInventory);
    Debug.Log(holderList.Count);
    foreach (var item in holderList)
    {
        if (item.ItemType == ItemType.CollectibleVase || item.ItemType == ItemType.CollectibleGoblet ||
            item.ItemType == ItemType.CollectibleBook)
        {
            continue;
        }

        GameObject itemFrame = Instantiate(inventoryEntry, inventoryParent.transform);

        inventoryItems.Add(itemFrame);

        var entry = itemFrame.GetComponent<LobbyInventoryChooseEntry>();
        entry.parent = this;
        entry.index = i;
        entry.itemIcon.sprite = item.ItemIcon;
        i++;
        var itemData = inventory.InventoryLobby[entry.index];
        if (itemData.ProceedToDungeon)
        {
            ItemSelected(true, entry, itemData);
        }
    }

    ReloadRefresh();
}

public void ReloadRefresh()
{
    foreach (var item in inventoryItems)
    {
        Destroy(item.gameObject);
    }

    inventoryItems.Clear();

    int i = 0;

    var holderList = inventory.InventoryLobby;
    holderList.AddRange(inventory.ItemsInInventory);
    Debug.Log(holderList.Count);
    foreach (var item in holderList)
    {
        if (item.ItemType == ItemType.CollectibleVase || item.ItemType == ItemType.CollectibleGoblet ||
            item.ItemType == ItemType.CollectibleBook)
        {
            continue;
        }

        GameObject itemFrame = Instantiate(inventoryEntry, inventoryParent.transform);

        inventoryItems.Add(itemFrame);

        var entry = itemFrame.GetComponent<LobbyInventoryChooseEntry>();
        entry.parent = this;
        entry.index = i;
        entry.itemIcon.sprite = item.ItemIcon;
        i++;
        var itemData = inventory.InventoryLobby[entry.index];
        if (itemData.ProceedToDungeon)
        {
            ItemSelected(true, entry, itemData);
        }
    }
}

public void ItemSelected(bool isSelected, LobbyInventoryChooseEntry entry, ItemData itemData)
{
    entry.isSelected = !isSelected;
    itemData.ProceedToDungeon = !isSelected;
    if (!isSelected)
    {
        entry.frame.sprite = frameActive;
    }
    else
    {
        entry.frame.sprite = frameInactive;
    }

    if (entry.isSelected)
    {
        inventory.ItemsInInventory.Add(itemData);
        inventory.InventoryLobby.Remove(itemData);
    }
    else
    {
        inventory.ItemsInInventory.Remove(itemData);
        inventory.InventoryLobby.Add(itemData);
    }
}*/

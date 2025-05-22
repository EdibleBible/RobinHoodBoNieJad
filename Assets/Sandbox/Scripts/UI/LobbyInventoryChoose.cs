using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInventoryChoose : MonoBehaviour
{
    public GameObject inventoryEntry;
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
        foreach (var item in inventory.InventoryLobby)
        {
            if (item.ItemType == ItemType.CollectibleVase || item.ItemType == ItemType.CollectibleGoblet || item.ItemType == ItemType.CollectibleBook)
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
            if (itemData.ProceedToDungeon) { ItemSelected(true, entry, itemData); }
        }
    }

    public void ItemSelected(bool isSelected, LobbyInventoryChooseEntry entry, ItemData itemData)
    {
        entry.isSelected = !isSelected;
        itemData.ProceedToDungeon = !isSelected;
        if (!isSelected) { entry.frame.sprite = frameActive; }
        else { entry.frame.sprite = frameInactive; }
    }
}


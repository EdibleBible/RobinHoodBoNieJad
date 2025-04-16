using System.Collections.Generic;
using UnityEngine;

public class LobbyInventoryChoose : MonoBehaviour
{
    public GameObject inventoryEntry;
    public SOInventory inventory;
    public GameObject inventoryParent;
    public List<GameObject> inventoryItems = new();

    private void OnEnable()
    {
        ReloadInventory();
    }

    private void OnDisable()
    {
        foreach (var item in inventoryItems)
        {
            Destroy(item.gameObject);
        }
        inventoryItems.Clear();
    }



    public void ReloadInventory()
    {
        foreach (var item in inventory.InventoryLobby)
        {
            GameObject itemFrame = Instantiate(inventoryEntry, inventoryParent.transform);
            inventoryItems.Add(itemFrame);
            var entry = itemFrame.GetComponent<LobbyInventoryChooseEntry>();
            var itemData = inventory.InventoryLobby[entry.index];
            if (itemData.ProceedToDungeon)
            {
                entry.selected = true;
            }
        }
    }
}


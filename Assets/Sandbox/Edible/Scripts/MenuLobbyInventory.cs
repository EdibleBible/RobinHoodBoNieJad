using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class MenuLobbyInventory : MonoBehaviour
{
    public SOInventory inventory;
    public List<MenuLobbyInventoryEntry> entryList = new();
    public List<ItemBase> itemList = new();
    public int inventoryPage;
    public GameObject entryPrefab;
    public Transform panel1;
    public Transform panel2;
    public int pagesCount;

    private void Start()
    {
        itemList = inventory.itemList;
        if (itemList != null)
        {
            ReloadInventory();
        }
        pagesCount = Mathf.CeilToInt(itemList.Count / 10);
    }

    private void ReloadInventory()
    {
        foreach(var entry in entryList)
        {
            Destroy(entry.gameObject);
        }
        entryList.Clear();
        int upperCap = itemList.Count % 10;
        for (int i = 0 + (10 * inventoryPage); i < upperCap + (10 * inventoryPage); i++)
        {
            Transform parentPanel;
            if (i < 5 + (10 * inventoryPage))
            {
                parentPanel = panel1;
            }
            else
            {
                parentPanel = panel2;
            }
            MenuLobbyInventoryEntry newEntry = Instantiate(entryPrefab, parentPanel.transform).GetComponent<MenuLobbyInventoryEntry>();
            entryList.Add(newEntry);
            newEntry.LoadItem(itemList[i]);
        }
    }

    private void ChangePage(int value)
    {
        if ((value == -1 && inventoryPage == 0)|| (value == 1 && inventoryPage == pagesCount))
        {

        }
    }
}

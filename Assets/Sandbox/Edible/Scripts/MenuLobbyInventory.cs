using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class MenuLobbyInventory : MonoBehaviour
{
    public SOInventory inventory;
    public List<MenuLobbyInventoryEntry> entryList = new();
    public List<ItemBase> itemList = new();
    public int inventoryPage;
    public GameObject entryPrefab;
    public Transform panel1;
    public Transform panel2;
    private int pagesCount;
    public TMP_Text pagesText;
    public TMP_Text coinsText;

    private void Start()
    {
        IndexInventory(); 
        if (itemList != null)
        {
            ReloadInventory();
        }
        coinsText.text = inventory.playerScore.ToString();
        pagesText.text = (inventoryPage + 1).ToString() + "/" + (pagesCount + 1).ToString();
    }

    private void IndexInventory()
    {
        itemList = inventory.itemList;
        pagesCount = Mathf.FloorToInt(itemList.Count / 10);
    }

    private void ReloadInventory()
    {
        foreach(var entry in entryList)
        {
            Destroy(entry.gameObject);
        }
        entryList.Clear();
        int upperCap = 10;
        if (pagesCount == inventoryPage) {upperCap = itemList.Count % 10;}
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
            newEntry.entryIndex = i;
            newEntry.lobbyInventory = this;
        }
    }

    public void ChangePage(int value)
    {
        if ((value == -1 && inventoryPage > 0) || (value == 1 && inventoryPage < pagesCount))
        {
            inventoryPage += value;
            ReloadInventory();
            pagesText.text = (inventoryPage+1).ToString() + "/" + (pagesCount+1).ToString();
        }
    }

    public void Sell(int index)
    {
        ItemBase item = itemList[index];
        inventory.playerScore += item.itemValue;
        coinsText.text = inventory.playerScore.ToString();
        itemList.RemoveAt(index);
        IndexInventory();
        if (index + 1 == itemList.Count && index != 0)
        {
            ChangePage(-1);
        }
        pagesText.text = (inventoryPage + 1).ToString() + "/" + (pagesCount + 1).ToString();
        ReloadInventory();
    }
}

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class MenuLobbyInventory : MonoBehaviour
{
    public SOInventory inventory;
    public List<MenuLobbyInventoryEntry> entryList = new();
    public List<ItemData> itemList = new();
    public int inventoryPage;
    public GameObject entryPrefab;
    public Transform panel1;
    public Transform panel2;
    private int pagesCount;
    public TMP_Text pagesText;
    public TMP_Text coinsText;
    [SerializeField] private List<ShopCellController> shopCellControllers = new();

    private void OnEnable()
    {
        IndexInventory(); 
        if (itemList != null)
        {
            ReloadInventory();
        }
        coinsText.text = inventory.CurrInvenoryScore.ToString();
        pagesText.text = (inventoryPage + 1).ToString() + "/" + (pagesCount + 1).ToString();
    }

    private void IndexInventory()
    {
        itemList = inventory.ItemsInInventory;
        pagesCount = Mathf.FloorToInt(itemList.Count / 10);
    }

    private void ReloadInventory()
    {
        foreach(var element in shopCellControllers)
        {
            element.ChangeShowElement(false);
        }
        foreach(var entry in entryList)
        {
            Destroy(entry.gameObject);
        }
        entryList.Clear();
        int upperCap = 10;
        if (pagesCount == inventoryPage) {upperCap = itemList.Count % 10;}
        for (int i = 0 + (10 * inventoryPage); i < upperCap + (10 * inventoryPage); i++)
        {
            var emptyCell = shopCellControllers.Where(x => !x.ShowElement).FirstOrDefault();
            MenuLobbyInventoryEntry newEntry = Instantiate(entryPrefab, emptyCell.ItemHolder).GetComponent<MenuLobbyInventoryEntry>();
            emptyCell.ShowElement = true;
            newEntry.gameObject.transform.localPosition = new Vector3(0, 0, 0);
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
        ItemData item = itemList[index];
        inventory.CurrInvenoryScore += item.ItemValue;
        coinsText.text = inventory.CurrInvenoryScore.ToString();
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

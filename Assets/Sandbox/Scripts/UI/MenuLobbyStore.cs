using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class MenuLobbyStore : MonoBehaviour
{
    public SOStore store;
    public SOInventory inventory;
    public List<MenuLobbyStoreEntry> entryList = new();
    public List<ItemData> itemList = new();
    public int inventoryPage;
    public GameObject entryPrefab;
    public Transform panel1;
    public Transform panel2;
    private int pagesCount;
    public TMP_Text pagesText;
    public SOStats playerStats;
    public SOInventory playerInventory;

    [SerializeField] private List<ShopCellController> shopCellControllers = new();

    private void Awake()
    {
        int maxVisit = 1;
        int visit = playerStats.lobbyVisit;
        foreach (StoreEntry entry in store.storeEntries) //Finds last visit # of an offer
        {
            if (entry.keyVisit > maxVisit) { maxVisit = entry.keyVisit; }
        }
        if (playerStats.lobbyVisit > maxVisit) //Loops the # of visit to cycle offers
        {
            visit = playerStats.lobbyVisit % maxVisit;
        }
        foreach (StoreEntry entry in store.storeEntries)  //Finds the current looped cycle
        {
            if (entry.keyVisit == visit)
            {
                GameObject newObject = Instantiate(entry.itemPrefab);
                store.storeItems.Add(newObject.GetComponent<ItemBase>().ItemData);
                Destroy(newObject);
            }
        }
    }

    private void OnEnable()
    {
        IndexInventory();
        if (itemList != null)
        {
            ReloadInventory();
        }
        pagesText.text = (inventoryPage + 1).ToString() + "/" + (pagesCount + 1).ToString();
    }

    private void IndexInventory()
    {
        itemList = store.storeItems;
        pagesCount = Mathf.FloorToInt(itemList.Count / 10);
    }

    private void ReloadInventory()
    {
        foreach (var entry in entryList)
        {
            Destroy(entry.gameObject);
        }
        entryList.Clear();
        int upperCap = 10;
        if (pagesCount == inventoryPage) { upperCap = itemList.Count % 10; }
        for (int i = 0 + (10 * inventoryPage); i < upperCap + (10 * inventoryPage); i++)
        {
            var emptyCell = shopCellControllers.Where(x => !x.ShowElement).FirstOrDefault();
            MenuLobbyStoreEntry newEntry = Instantiate(entryPrefab, emptyCell.transform).GetComponent<MenuLobbyStoreEntry>();
            emptyCell.ShowElement = true;
            newEntry.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            entryList.Add(newEntry);
            newEntry.LoadItem(itemList[i]);
            newEntry.entryIndex = i;
            newEntry.lobbyStore = this;
        }
    }

    public void ChangePage(int value)
    {
        if ((value == -1 && inventoryPage > 0) || (value == 1 && inventoryPage < pagesCount))
        {
            inventoryPage += value;
            ReloadInventory();
            pagesText.text = (inventoryPage + 1).ToString() + "/" + (pagesCount + 1).ToString();
        }
    }

    public void Buy(int index)
    {
        ItemData item = itemList[index];
        if (inventory.CurrInvenoryScore < item.ItemValue)
        {
            Debug.Log("d0pa");
            return;
        }
        playerInventory.AddItemToInventory(item);
        inventory.CurrInvenoryScore -= item.ItemValue;
        playerStats.scoreTotal += (int)item.ItemValue;
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

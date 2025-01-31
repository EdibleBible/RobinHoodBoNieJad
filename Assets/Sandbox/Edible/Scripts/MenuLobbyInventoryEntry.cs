using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuLobbyInventoryEntry : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemValueText;
    public int entryIndex;
    public MenuLobbyInventory lobbyInventory;

    public void LoadItem(ItemBase item)
    {
        itemIcon.sprite = item.itemIcon;
        itemName.text = item.itemName;
        itemValueText.text = item.itemValue.ToString();
    }

    public void Sell()
    {
        lobbyInventory.Sell(entryIndex);
    }
}

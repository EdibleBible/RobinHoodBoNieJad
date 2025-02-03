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

    public void LoadItem(ItemData item)
    {
        itemIcon.sprite = item.ItemIcon;
        itemName.text = item.ItemName;
        itemValueText.text = item.ItemValue.ToString();
    }

    public void Sell()
    {
        lobbyInventory.Sell(entryIndex);
    }
}

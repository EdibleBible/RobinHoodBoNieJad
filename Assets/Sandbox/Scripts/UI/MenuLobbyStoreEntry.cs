using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuLobbyStoreEntry : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemValueText;
    public int entryIndex;
    public MenuLobbyStore lobbyStore;

    public void LoadItem(ItemData item)
    {
        itemIcon.sprite = item.ItemIcon;
        itemName.text = item.ItemName;
        itemValueText.text = item.ItemValue.ToString();
    }

    public void Buy()
    {
        lobbyStore.Buy(entryIndex);
    }
}

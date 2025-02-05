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

    public void LoadItem(ItemModifier item)
    {
        itemIcon.sprite = item.Icon;
        itemName.text = item.Name;
        itemValueText.text = item.Value.ToString();
    }

    public void Buy()
    {
        lobbyStore.Buy(entryIndex);
    }
}

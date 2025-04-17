using UnityEngine;
using UnityEngine.UI;

public class LobbyInventoryChooseEntry : MonoBehaviour
{
    public Image itemIcon;
    public Image frame;
    public int index;
    public bool isSelected;
    public Button button;
    public LobbyInventoryChoose parent;

    public void SelectButton()
    {
        parent.ItemSelected(isSelected, this, parent.inventory.InventoryLobby[index]);
    }
}

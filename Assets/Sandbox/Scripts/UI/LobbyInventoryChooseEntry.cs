using UnityEngine;
using UnityEngine.UI;

public class LobbyInventoryChooseEntry : MonoBehaviour
{
    public Image itemIcon;
    public Image frame;
    public ItemData data;
    public bool isSelected;
    public Button button;
    public LobbyInventoryChoose parent;

    public void SelectButton()
    {
        parent.SelectFrame(isSelected, this, data);
    }
}

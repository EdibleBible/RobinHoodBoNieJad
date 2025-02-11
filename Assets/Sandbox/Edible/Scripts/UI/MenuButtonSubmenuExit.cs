using UnityEngine;

public class MenuButtonSubmenuExit : MonoBehaviour
{
    public GameObject menu;
    public GameObject selector;

    public void OpenMenu()
    {
        menu.SetActive(true);
    }
    public void CloseMenu()
    {
        selector.gameObject.SetActive(false);
        menu.SetActive(false);
    }
}

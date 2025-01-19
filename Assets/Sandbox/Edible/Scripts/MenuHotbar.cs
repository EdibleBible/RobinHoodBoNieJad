using UnityEngine;
using UnityEngine.UI;

public class MenuHotbar : MonoBehaviour
{
    public MenuHotbarEntry hotbarHandEntry;
    public Slider progressBar;

    void Awake()
    {
        PlayerHotbar.GetHotbarTransform += ReturnTransform;
    }

    MenuHotbar ReturnTransform()
    {
        PlayerHotbar.GetHotbarTransform -= ReturnTransform;
        return this;
    }
}

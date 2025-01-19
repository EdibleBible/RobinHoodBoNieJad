using UnityEngine;

public class MenuHotbar : MonoBehaviour
{
    public MenuHotbarEntry hotbarHandEntry;

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

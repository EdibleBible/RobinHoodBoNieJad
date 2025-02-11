using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInventory : MonoBehaviour
{
    public List<Image> itemIcons = new();

    public void UpdateItemIcon(int iconInt, Sprite itemIcon)
    {
        itemIcons[iconInt].sprite = itemIcon;
    }

    public void ClearInventoryIcons()
    {
        foreach (var item in itemIcons)
        {
            item.sprite = null;
        }
    }
}

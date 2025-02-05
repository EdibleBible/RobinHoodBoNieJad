using System;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ShopCellController : MonoBehaviour
{
    public bool ShowElement;
    public Transform ItemHolder;

    private void OnDisable()
    {
        ShowElement = false;
    }

    public void ChangeShowElement(bool data)
    {
        ShowElement = data;
    }
}
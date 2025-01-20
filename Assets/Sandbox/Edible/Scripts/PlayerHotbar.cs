using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHotbar : MonoBehaviour
{
    [HideInInspector] public List<ItemBase> itemList = new();
    [HideInInspector] public List<MenuHotbarEntry> hotbarEntryList = new();
    [HideInInspector] public PlayerBase playerBase;
    public int size;
    public int sizeTaken;
    public int currentItemIndex = 0;
    [HideInInspector] public ItemBase currentItemBase;
    public InputActionAsset globalInputActions;
    private InputAction dropAction;
    private InputAction scrollAction;
    private MenuHotbar hotbar;
    private Transform hotbarUIPanel;
    private Slider hotbarProgressBar;
    public GameObject hotbarEntryPrefab;
    public Transform itemDropSpot;
    public delegate MenuHotbar GetHotbarEvent();
    public static event GetHotbarEvent GetHotbar;

    private void Start()
    {
        hotbar = GetHotbar();
        hotbarUIPanel = hotbar.gameObject.transform;
        hotbarEntryList.Add(hotbar.hotbarHandEntry);
        hotbarProgressBar = hotbar.progressBar;
        hotbar.SetSize(size);
    }

    public void SaveToInventory(SOInventory inventory)
    {
        if (itemList.Count > 1)
        {
            for (int i = 1; i < itemList.Count; i++)
            {
                inventory.itemList.Add(itemList[i]);
            }
        }
    }

    public void LoadFromInventory(SOInventory inventory)
    {
        for(int i = 0; i < inventory.itemList.Count;i++)
        {
            Index(inventory.itemList[i]);
        }
    }

    public void Clear()
    {
        currentItemIndex = 0;
        currentItemBase = null;
        if (itemList.Count > 1)
        {
            for (int i = 1; i < itemList.Count; i++)
            {
                Deindex(i);
            }
        }
    }

    public void Index(ItemBase item)
    {
        itemList.Add(item);
        GameObject newItem = Instantiate(hotbarEntryPrefab, hotbarUIPanel, false);
        MenuHotbarEntry itemEntry = newItem.GetComponent<MenuHotbarEntry>();
        itemEntry.image.sprite = item.itemIcon;
        itemEntry.text.text = item.itemName;
        hotbarEntryList.Add(itemEntry);
    }

    public void Deindex(int index)
    {
        Destroy(hotbarEntryList[index].gameObject);
        hotbarEntryList.RemoveAt(index);
        itemList.RemoveAt(index);
    }

    public void Resize(int newSize)
    {
        size = newSize;
        hotbar.SetSize(newSize);
    }

    public bool IsHotbarFull(int itemSize)
    {
        if (size < sizeTaken + itemSize)
        {
            return true;
        }
        return false;
    }

    public bool PickUp(ItemBase item)
    {
        if (!IsHotbarFull(item.itemSize))
        {
            Index(item);
            sizeTaken += item.itemSize;
            hotbar.SetValue(sizeTaken);
            ModPlayer(item, true);
            return true;
        }
        return false;
    }

    /*public void Drop(ItemBase item)
    {
        int itemIndex = itemList.IndexOf(item);
        Destroy(hotbarEntryList[itemIndex].gameObject);
        hotbarEntryList.RemoveAt(itemIndex);
        itemList.Remove(item);
    }*/

    public void Drop(int itemIndex)
    {
        HotbarScroll(-1);
        GameObject itemObject = itemList[itemIndex].gameObject;
        itemObject.SetActive(true);
        itemObject.transform.position = itemDropSpot.position;
        itemObject.transform.SetParent(null);
        ItemBase item = itemObject.GetComponent<ItemBase>();
        item.canInteract = true;
        sizeTaken -= item.itemSize;
        hotbarProgressBar.value = sizeTaken;
        Deindex(itemIndex);
        ModPlayer(item, false);
    }

    public void ModPlayer(ItemBase item, bool toAdd)
    {
        int boolMultiplier;
        if (toAdd) { boolMultiplier = 1; } else { boolMultiplier = -1; };
        if (item.itemAttHotbarSizeMod != 0)
        {
            size += item.itemAttHotbarSizeMod * boolMultiplier;
            hotbarProgressBar.maxValue = size;
        }
    }

    private void Awake()
    {
        dropAction = globalInputActions.FindAction("Drop");
        dropAction.Enable();
        dropAction.started += HandleDrop;
        scrollAction = globalInputActions.FindAction("Scroll");
        scrollAction.Enable();
        scrollAction.started += HandleScroll;
    }

    private void OnDisable()
    {
        dropAction.started -= HandleDrop;
        dropAction.Disable();
        scrollAction.started -= HandleScroll;
        scrollAction.Disable();
    }

    private void HandleDrop(InputAction.CallbackContext context)
    {
        if (currentItemIndex != 0)
        {
            int itemSize = currentItemBase.itemSize;
            int sizeMod = currentItemBase.itemAttHotbarSizeMod;
            if (size - sizeMod >= sizeTaken - itemSize)
            {
                Drop(currentItemIndex);
            }
        }
    }

    private void HandleScroll(InputAction.CallbackContext context)
    {
        if (itemList.Count > 0)
        {
            float scrollValue = context.ReadValue<float>();
            if (scrollValue < 0)
            {
                HotbarScroll(1);
            }
            else if (scrollValue > 0)
            {
                HotbarScroll(-1);
            }
        }
    }

    private void HotbarScroll(int value)
    {
        hotbarEntryList[currentItemIndex].SwitchSelector(false);
        currentItemIndex += value;
        if (currentItemIndex < 0)
        {
            currentItemIndex = itemList.Count - 1;
        } 
        else if (currentItemIndex >= itemList.Count)
        {
            currentItemIndex = 0;
        }
        currentItemBase = itemList[currentItemIndex].GetComponent<ItemBase>();
        hotbarEntryList[currentItemIndex].SwitchSelector(true);
    }
}

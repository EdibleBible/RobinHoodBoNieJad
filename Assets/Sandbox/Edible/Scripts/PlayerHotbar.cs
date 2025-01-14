using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHotbar : MonoBehaviour
{
    public List<ItemBase> itemList = new();
    public List<MenuHotbarEntry> hotbarEntryList = new();
    [HideInInspector] public PlayerBase playerBase;
    public int size;
    public int sizeTaken;
    public int currentItemIndex = 0;
    public ItemBase currentItemBase;
    public InputActionAsset globalInputActions;
    private InputAction dropAction;
    private InputAction scrollAction;
    public Transform hotbarUIPanel;
    public GameObject hotbarEntryPrefab;
    public Transform itemDropSpot;

    public void Resize(int newSize)
    {
        size = newSize;
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
            itemList.Add(item);
            GameObject newItem = Instantiate(hotbarEntryPrefab, hotbarUIPanel, false);
            MenuHotbarEntry itemEntry = newItem.GetComponent<MenuHotbarEntry>();
            hotbarEntryList.Add(itemEntry);
            itemEntry.image.sprite = item.itemIcon;
            itemEntry.text.text = item.itemName;
            sizeTaken += item.itemSize;
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
        Destroy(hotbarEntryList[itemIndex].gameObject);
        hotbarEntryList.RemoveAt(itemIndex);
        GameObject itemObject = itemList[itemIndex].gameObject;
        itemObject.SetActive(true);
        itemObject.transform.position = itemDropSpot.position;
        itemObject.transform.SetParent(null);
        ItemBase item = itemObject.GetComponent<ItemBase>();
        item.canInteract = true;
        sizeTaken -= item.itemSize;
        ModPlayer(item, false);
        itemList.RemoveAt(itemIndex);
    }

    public void ModPlayer(ItemBase item, bool toAdd)
    {
        int boolMultiplier;
        if (toAdd) { boolMultiplier = 1; } else { boolMultiplier = -1; };
        if (item.itemAttHotbarSizeMod != 0)
        {
            size += item.itemAttHotbarSizeMod * boolMultiplier;
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
        int itemSize = currentItemBase.itemSize;
        int sizeMod = currentItemBase.itemAttHotbarSizeMod;
        if (currentItemIndex != 0 && (size - sizeMod >= sizeTaken - itemSize))
        {
            Drop(currentItemIndex);
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHotbar : MonoBehaviour
{
    public List<ItemBase> itemList = new();
    public List<MenuHotbarEntry> hotbarEntryList = new();
    [HideInInspector] public PlayerBase playerBase;
    public int size;
    public int currentItemIndex = 0;
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

    public bool IsHotbarFull()
    {
        return false;
    }

    public bool PickUp(ItemBase item)
    {
        if (!IsHotbarFull())
        {
            itemList.Add(item);
            GameObject newItem = Instantiate(hotbarEntryPrefab, hotbarUIPanel, false);
            MenuHotbarEntry itemEntry = newItem.GetComponent<MenuHotbarEntry>();
            hotbarEntryList.Add(itemEntry);
            itemEntry.image.sprite = item.itemIcon;
            itemEntry.text.text = item.itemName;
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
        itemObject.GetComponent<ItemBase>().canInteract = true;
        itemList.RemoveAt(itemIndex);
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
        hotbarEntryList[currentItemIndex].SwitchSelector(true);
    }
}

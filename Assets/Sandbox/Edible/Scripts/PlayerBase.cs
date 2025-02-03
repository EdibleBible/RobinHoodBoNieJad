using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private Transform dropPointTransform;
    [SerializeField] private SOInventory playerInventory;
    [SerializeField] private GameEvent InventorySetUpEvent;
    [SerializeField] private GameEvent DropItemEvent;
    [SerializeField] private GameEvent PickupItemEvent;
    [SerializeField] private GameEvent InventoryUpdateSelectedItemEvent;
    private int currentSelectedItem = 0;

    public ItemData CurrSelectedItem;

    public void Awake()
    {
        playerInventory.ClearInventory();
        playerInventory.SetUpInventory();
        InventorySetUpEvent?.Raise(this, playerInventory);
    }

    private void Update()
    {
        if (CurrSelectedItem == null)
        {
            Debug.LogWarning("No item selected");
        }
        else
        {
            Debug.Log(CurrSelectedItem.ItemName);
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
        {
            UpdateSelectedSlot(-1); // Scroll w górę
        }
        else if (scrollInput < 0f)
        {
            UpdateSelectedSlot(1); // Scroll w dół
        }
    }

    private void UpdateSelectedSlot(int direction)
    {
        if (currentSelectedItem + direction < 0 ||
            currentSelectedItem + direction >= playerInventory.InventorySize) return;
        (int curr, int prev) data = (currentSelectedItem + direction, currentSelectedItem);
        currentSelectedItem = data.curr;
        InventoryUpdateSelectedItemEvent?.Raise(this, data);
    }

    public bool PickUp(ItemData itemBase)
    {
        if (playerInventory.AddItemToInventory(itemBase))
        {
            PickupItemEvent?.Raise(this, itemBase);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DropItem()
    {
        Debug.Log("Drop 2");
        if (CurrSelectedItem != null)
        {
            Debug.Log("Drop 3");
            var obj = Instantiate(CurrSelectedItem.ItemPrefab);
            obj.transform.position = dropPointTransform.position;
            obj.transform.rotation = quaternion.identity;
            playerInventory.RemoveItemFromInventory(CurrSelectedItem);
            DropItemEvent?.Raise(this, currentSelectedItem);

        }
    }
}
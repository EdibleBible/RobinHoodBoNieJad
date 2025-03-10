using System;
using System.Collections.Generic;
using Script.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBase : MonoBehaviour
{
    public Camera camera;
    
    [SerializeField] private Transform dropPointTransform;
    public SOInventory PlayerInventory;
    [SerializeField] private GameEvent InventorySetUpEvent;
    [SerializeField] private GameEvent DropItemEvent;
    [SerializeField] private GameEvent PickupItemEvent;
    [SerializeField] private GameEvent InventoryUpdateSelectedItemEvent;
    private int currentSelectedItem = 0;

    public SOPlayerStatsController PlayerStatsController;
    
    [HideInInspector]public ItemData CurrSelectedItem;

    public void Start()
    {
        PlayerInventory.ClearInventory();
        PlayerInventory.SetUpInventory();
        InventorySetUpEvent?.Raise(this, PlayerInventory);
    }

    private void Update()
    {
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
            currentSelectedItem + direction >= PlayerInventory.InventorySize) return;
        (int curr, int prev) data = (currentSelectedItem + direction, currentSelectedItem);
        currentSelectedItem = data.curr;
        InventoryUpdateSelectedItemEvent?.Raise(this, data);
    }

    public bool PickUp(ItemData itemBase)
    {
        if (PlayerInventory.AddItemToInventory(itemBase))
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
        if (CurrSelectedItem != null)
        {
            var obj = Instantiate(CurrSelectedItem.ItemPrefab);
            obj.transform.position = dropPointTransform.position;
            obj.transform.rotation = quaternion.identity;
            PlayerInventory.RemoveItemFromInventory(CurrSelectedItem);
            CurrSelectedItem.RemoveModifier(PlayerStatsController);
            DropItemEvent?.Raise(this, currentSelectedItem);
        }
    }

    public void RemoveItemFromInventory(ItemData itemToRemove)
    {
        PlayerInventory.RemoveItemFromInventory(itemToRemove);
        DropItemEvent?.Raise(this, itemToRemove);
    }
}
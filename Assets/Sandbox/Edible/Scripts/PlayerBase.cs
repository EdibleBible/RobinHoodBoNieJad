using System;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private SOInventory playerInventory;
    [SerializeField] private GameEvent InventorySetUpEvent;
    [SerializeField] private GameEvent DropItemEvent;
    [SerializeField] private GameEvent InventoryUpdateSelectedItemEvent;
    private int currentSelectedItem = 0;
    public void Awake()
    {
        playerInventory.ClearInventory();
        playerInventory.SetUpInventory();
        InventorySetUpEvent?.Raise(this,playerInventory);
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
        if (currentSelectedItem + direction < 0 || currentSelectedItem + direction >= playerInventory.InventorySize) return;
        (int curr, int prev) data = (currentSelectedItem + direction, currentSelectedItem);
        currentSelectedItem = data.curr;
        InventoryUpdateSelectedItemEvent?.Raise(this,data);
    }

    public bool PickUp(ItemData itemBase)
    {
        return playerInventory.AddItemToInventory(itemBase);
        
    }

    public void DropItem()
    {
        DropItemEvent?.Raise(this,currentSelectedItem);
    }
}

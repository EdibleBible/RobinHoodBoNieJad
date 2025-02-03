using System;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private SOInventory playerInventory;
    [SerializeField] private GameEvent InventorySetUpEvent;

    public void Awake()
    {
        playerInventory.ClearInventory();
        playerInventory.SetUpInventory();
        InventorySetUpEvent?.Raise(this,playerInventory);
    }
    public bool PickUp(ItemData itemBase)
    {
        return playerInventory.AddItemToInventory(itemBase);
        
    }
}

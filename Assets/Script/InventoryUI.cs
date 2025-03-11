using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    [SerializeField] private ItemSlot itemSlotPrefab;
    [SerializeField] private Transform itemSlotParent;
    
    [SerializeField] private GameEvent DropItemEvent;
    
    public void SetUpInventory(Component sender, object data)
    {
        foreach (Transform child in itemSlotParent)
        {
            Destroy(child.gameObject);
        }

        itemSlots.Clear(); // Resetujemy listę slotów

        if (sender is PlayerBase && data is SOInventory inventoryData)
        {
            for (int i = 0; i < inventoryData.CurrInventorySize; i++)
            {
                var slot = Instantiate(itemSlotPrefab, itemSlotParent);
                slot.SetUpSlot(i, null, i == 0);
                itemSlots.Add(slot);
            }
        }
    }

    public void AddItemToUI(Component sender, object data)
    {
        if (data is ItemData itemData && sender is PlayerBase playerBase)
        {
            if (itemData.ItemSize == 0)
            {
                return;
            }
            else if (itemData.ItemSize == 1)
            {
                var firstEmptySlot = itemSlots.FirstOrDefault(x => x.AssignedItem == null);
                if (firstEmptySlot != null)
                {
                    firstEmptySlot.AssignItem(itemData, false);
                }

                if (firstEmptySlot.IsSelected)
                {
                    playerBase.CurrSelectedItem = firstEmptySlot.AssignedItem;
                }
            }
            else
            {
                List<ItemSlot> requiredSlot = new List<ItemSlot>();
                for (int i = 0; i < itemData.ItemSize; i++)
                {
                    requiredSlot.Add(itemSlots.FirstOrDefault(x =>
                        x.AssignedItem == null && !requiredSlot.Contains(x)));
                }

                for (int i = 0; i < requiredSlot.Count; i++)
                {
                    if (i == 0)
                        requiredSlot[i].AssignItem(itemData, false);
                    else
                    {
                        requiredSlot[i].AssignItem(itemData, true);
                    }
                }

                var selectedSlot = requiredSlot.Where(x => x.IsSelected).FirstOrDefault();
                if (selectedSlot != null)
                {
                    playerBase.CurrSelectedItem = selectedSlot.AssignedItem;
                }
            }
        }
    }

    public void RemoveItemFromUI(Component sender, object data)
    {
        if (data is int slotToDropIndex && sender is PlayerBase playerBase)
        {
            var item = itemSlots[slotToDropIndex].AssignedItem;
            DropItemEvent.Raise(this,itemSlots[slotToDropIndex].AssignedItem);
            if (item.ItemSize == 1)
            {
                itemSlots[slotToDropIndex].RemoveAssignedItem();
                playerBase.CurrSelectedItem = null;
            }
            else
            {
                var allSlots = itemSlots.Where(x => x.AssignedItem == item);
                foreach (var slot in allSlots)
                {
                    slot.RemoveAssignedItem();
                }
            }
            
        }
        else if(data is ItemData itemData && sender is PlayerBase basePlayer)
        {
            var itemSlot = itemSlots.FirstOrDefault(x => x.AssignedItem == itemData);
            if (itemSlot.AssignedItem.ItemSize == 1)
            {
                itemSlot.RemoveAssignedItem();
                basePlayer.CurrSelectedItem = null;
            }
            else
            {
                var allSlots = itemSlots.Where(x => x.AssignedItem == itemSlot.AssignedItem);
                foreach (var slot in allSlots)
                {
                    slot.RemoveAssignedItem();
                }
            }
        }
    }

    public void UpdateSelectedSlot(Component sender, object data)
    {
        if (data is (int currSelected, int prevSelected) && sender is PlayerBase playerBase)
        {
            if (itemSlots == null || itemSlots.Count == 0)
                return;

            itemSlots[prevSelected].DeselectSlot();
            itemSlots[currSelected].SelectSlot();
            playerBase.CurrSelectedItem = itemSlots[currSelected].AssignedItem;
        }
    }
}
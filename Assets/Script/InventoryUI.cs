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

    public void InitializeInventoryUI(Component sender, object data)
    {
        Debug.LogError($"{sender.GetType().Name}.SetUpInventory");

        if (sender is PlayerBase playerBase && data is SOInventory inventoryData)
        {
            inventoryData.SetUpInventory();

            foreach (Transform child in itemSlotParent)
            {
                Destroy(child.gameObject);
            }

            itemSlots.Clear();

            for (int i = 0; i < inventoryData.CurrInventorySize; i++)
            {
                var slot = Instantiate(itemSlotPrefab, itemSlotParent);
                slot.SetUpSlot(i, null, i == playerBase.currentSelectedItem);
                itemSlots.Add(slot);
            }

            foreach (var item in inventoryData.ItemsInInventory)
            {
                AddItemToUI(playerBase, item);
            }
        }
    }

    public void SetUpInventory(Component sender, object data)
    {
        Debug.LogError($"{sender.GetType().Name}.SetUpInventory");
        if (sender is PlayerBase playerBase && data is SOInventory inventoryData)
        {
            Debug.Log($"inventory size: {inventoryData.CurrInventorySize}");
            List<ItemSlotSavedData> savedItemSlots = new List<ItemSlotSavedData>();
            for (int i = 0; i < itemSlots.Count; i++)
            {
                savedItemSlots.Add(new ItemSlotSavedData(itemSlots[i].SlotId, itemSlots[i].AssignedItem,
                    itemSlots[i].IsSelected));
            }

            foreach (Transform child in itemSlotParent)
            {
                Destroy(child.gameObject);
            }

            itemSlots.Clear();

            for (int i = 0; i < inventoryData.CurrInventorySize; i++)
            {
                var slot = Instantiate(itemSlotPrefab, itemSlotParent);
                slot.SetUpSlot(i, null, i == playerBase.currentSelectedItem);
                itemSlots.Add(slot);
            }

            int minCount = Mathf.Min(itemSlots.Count, savedItemSlots.Count);
            for (int i = 0; i < minCount; i++)
            {
                Debug.Log($"saved slot: {i}");
                itemSlots[i].SetUpSlot(savedItemSlots[i].SlotId, savedItemSlots[i].AssignedItem,
                    savedItemSlots[i].IsSelected);
            }
        }
    }

    public void AddItemToUI(Component sender, object data)
    {
        if (sender is PlayerBase playerBase && data is ItemData itemData)
        {
            if (itemData.ItemSize == 0)
            {
                return;
            }

            var firstEmptySlot = itemSlots.FirstOrDefault(x => x.AssignedItem == null);
            if (itemData.ItemSize == 1)
            {
                if (firstEmptySlot != null)
                {
                    firstEmptySlot.AssignItem(itemData, false);
                }
            }
            else
            {
                for (int i = 0; i < itemData.ItemSize; i++)
                {
                    if (i == 0)
                    {
                        if (firstEmptySlot != null)
                        {
                            firstEmptySlot.AssignItem(itemData, false);
                        }

                        continue;
                    }

                    firstEmptySlot = itemSlots.FirstOrDefault(x => x.AssignedItem == null);
                    firstEmptySlot.AssignItem(itemData, true);
                }
            }

            if (firstEmptySlot.IsSelected)
            {
                playerBase.CurrSelectedItem = firstEmptySlot.AssignedItem;
            }

            Debug.Log("AddItemToUI");
        }
        else
        {
            Debug.LogWarning("sender is null");
        }
    }

    public void RemoveItemFromUI(Component sender, object data)
    {
        if (data is int slotToDropIndex && sender is PlayerBase playerBase)
        {
            var item = itemSlots[slotToDropIndex].AssignedItem;
            DropItemEvent.Raise(this, itemSlots[slotToDropIndex].AssignedItem);
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
        else if (data is ItemData itemData && sender is PlayerBase basePlayer)
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
            Debug.Log($"Curr select item: {playerBase.CurrSelectedItem.ReturnData()}");
        }
    }

    private void Awake()
    {
    }
}

public struct ItemSlotSavedData
{
    public int SlotId;
    public ItemData AssignedItem;
    public bool IsSelected;

    public ItemSlotSavedData(int slotId, ItemData assignedItem, bool isSelected)
    {
        SlotId = slotId;
        AssignedItem = assignedItem;
        IsSelected = isSelected;
    }
}
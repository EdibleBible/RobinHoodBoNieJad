using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    [SerializeField] private ItemSlot itemSlotPrefab;
    [SerializeField] private Transform itemSlotParent;

    [SerializeField] private int currSelectedSlot = 0;

    public void SetUpInventory(Component sender, object data)
    {
        foreach (Transform child in itemSlotParent)
        {
            Destroy(child.gameObject);
        }

        itemSlots.Clear(); // Resetujemy listę slotów

        if (sender is PlayerBase && data is SOInventory inventoryData)
        {
            for (int i = 0; i < inventoryData.InventorySize; i++)
            {
                var slot = Instantiate(itemSlotPrefab, itemSlotParent);
                slot.SetUpSlot(i, null, i == currSelectedSlot);
                itemSlots.Add(slot);
            }
        }
    }

    public void AddItemToUI(Component sender, object data)
    {
        if (data is ItemData itemData)
        {
            if (itemData.ItemSize == 1)
            {
                var firstEmptySlot = itemSlots.FirstOrDefault(x => x.AssignedItem == null);
                if (firstEmptySlot != null)
                {
                    firstEmptySlot.AssignItem(itemData, false);
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
            }
        }
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
        if (itemSlots.Count == 0) return;

        if (currSelectedSlot + direction < 0 || currSelectedSlot + direction >= itemSlots.Count) return;

        itemSlots[currSelectedSlot].DeselectSlot();
        currSelectedSlot += direction;
        itemSlots[currSelectedSlot].SelectSlot();
    }
}
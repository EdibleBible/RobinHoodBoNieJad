using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public int SlotId;
    public ItemData AssignedItem;
    public bool IsSelected;

    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject itemIndicatorObject;

    [Header("Sprites")] [SerializeField] private Sprite backpackSprite;

    public void SetUpSlot(int slotID, ItemData assignedItem, bool isSelected)
    {
        SlotId = slotID;
        AssignedItem = assignedItem;

        if (AssignedItem != null)
        {
            itemImage.sprite = AssignedItem.ItemIcon;
            itemImage.color = new Color(255, 255, 255, 1);
        }
        else
        {
            itemImage.sprite = backpackSprite;
            itemImage.color = new Color(255, 255, 255, 0.2f);
        }

        if (isSelected)
        {
            itemIndicatorObject.SetActive(true);
            IsSelected = true;
        }
        else
        {
            itemIndicatorObject.SetActive(false);
            IsSelected = false;
        }
    }

    public void AssignItem(ItemData item, bool secondarySlot = false)
    {
        AssignedItem = item;
        itemImage.sprite = AssignedItem.ItemIcon;
        if (!secondarySlot)
        {
            itemImage.color = new Color(255, 255, 255, 1);
        }
        else
        {
            itemImage.color = new Color(255, 255, 255, 0.2f);
        }
    }

    public void RemoveAssignedItem()
    {
        AssignedItem = null;
        itemImage.sprite = backpackSprite;
        itemImage.color = new Color(255, 255, 255, 0.2f);
        Debug.Log(AssignedItem);
    }

    public void SelectSlot()
    {
        itemIndicatorObject.SetActive(true);
        IsSelected = true;
    }

    public void DeselectSlot()
    {
        itemIndicatorObject.SetActive(false);
        IsSelected = false;
    }
}
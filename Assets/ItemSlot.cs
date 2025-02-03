using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public int SlotId;
    public ItemData AssignedItem;
    
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject itemIndicatorObject;

    [Header("Sprites")] 
    [SerializeField] private Sprite backpackSprite;

    public void SetUpSlot(int slotID, ItemData assignedItem, bool isSelected)
    {
        SlotId = slotID;
        AssignedItem = assignedItem;
        
        if (AssignedItem != null)
        {
            itemImage.sprite = AssignedItem.itemIcon;
            itemImage.color = new Color(255, 255, 255, 1);
        }
        else
        {
            itemImage.sprite = backpackSprite;
            itemImage.color = new Color(255, 255, 255, 0.7f);
        }
        if (isSelected)
        {
            itemIndicatorObject.SetActive(true);
        }
        else
        {
            itemIndicatorObject.SetActive(false);
        }
    }
    
    public void AssignItem(ItemData item, bool secondarySlot = false)
    {
        AssignedItem = item;
        itemImage.sprite = AssignedItem.itemIcon;
        if (!secondarySlot)
        {
            itemImage.color = new Color(255, 255, 255, 1);
        }
        else
        {
            itemImage.color = new Color(255, 255, 255, 0.7f);
        }
    }

    public void RemoveAssignedItem()
    {
        AssignedItem = null;
    }
    
    public void SelectSlot()
    {
        itemIndicatorObject.SetActive(true);
    }

    public void DeselectSlot()
    {
        itemIndicatorObject.SetActive(false);
    }
    
}

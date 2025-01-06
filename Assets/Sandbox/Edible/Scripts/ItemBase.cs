using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public enum ItemType {Debug};
    public List<int> itemTypeValues = new() {100};
    public static event System.Action<ItemBase> OnItemAdded;
    public ItemType itemType;
    public int itemSize;
    public Sprite itemIcon;

    public void PickUp()
    {
        OnItemAdded?.Invoke(this);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour, IInteract
{
    public enum ItemType {Debug};
    public List<int> itemTypeValues = new() {100};
    public static event System.Action<ItemBase> OnItemAdded;
    public ItemType itemType;
    public string itemName;
    public int itemSize;
    public Sprite itemIcon;
    public bool canInteract = true;

    public bool Interact(PlayerBase playerBase)
    {
        if (canInteract && playerBase.PickUp(this)){
            gameObject.SetActive(false);
            gameObject.transform.parent = playerBase.transform;
            canInteract = false;
            return true;
        }
        return false;
    }
}

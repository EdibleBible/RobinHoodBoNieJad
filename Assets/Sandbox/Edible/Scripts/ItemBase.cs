using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour, IInteract
{
    public enum ItemType {Debug, CollectibleVase, CollectibleGoblet, UtilityBackpack};
    [HideInInspector] public List<int> itemTypeValues = new() {100, 200, 75, 0};
    public static event System.Action<ItemBase> OnItemAdded;
    public ItemType itemType;
    public string itemName;
    public string itemDescription;
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

    [Header("Item Attributes")]
    public int itemAttHotbarSizeMod;
}

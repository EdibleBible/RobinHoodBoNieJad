using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour, IInteractable
{
    [Header("Item Attributes")] public ItemType ItemType;
    public string ItemName;
    public string ItemDescription;
    public int ItemSize;
    public Sprite itemIcon;

    [HideInInspector] public bool CanInteract { get; set; } = true;
    [HideInInspector] public bool IsBlocked { get; set; } = false;
    [HideInInspector] public bool TwoSideInteraction { get; set; } = false;

    [Header("Events")] [SerializeField] private GameEvent showUIEvent;
    [SerializeField] private GameEvent interactEvent;

    public GameEvent ShowUIEvent
    {
        get => showUIEvent;
        set => showUIEvent = value;
    }

    public GameEvent InteractEvent
    {
        get => interactEvent;
        set => interactEvent = value;
    }

    [Header("Callbacks")]
    public string InteractMessage
    {
        get => interactMessage;
        set => interactMessage = value;
    }

    [SerializeField] private string interactMessage;

    public string BlockedMessage
    {
        get => blockedMessage;
        set => blockedMessage = value;
    }

    [SerializeField] private string blockedMessage;

    public void Interact(Transform player)
    {
        if (!CanInteract || IsBlocked)
        {
            ShowUIEvent.Raise(this, (true, BlockedMessage, true));
            return;
        }

        //POTEM TO ZMIEŃ
        PlayerBase playerBase = player.GetComponent<PlayerBase>();

        if (playerBase == null)
        {
            Debug.LogError("Player is not a PlayerBase");
            return;
        }

        if (playerBase.PickUp(this))
            Destroy(gameObject);
        else
        {
            Debug.LogWarning("item is not picked up");
        }
    }

    public void ShowUI()
    {
        ShowUIEvent.Raise(this, (true, InteractMessage, false));
    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this, (false, "", false));
    }
    
    

    /*public enum ItemType {};
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
    public int itemValue;
    public int itemAttHotbarSizeMod;*/
}

public enum ItemType
{
    Debug,
    CollectibleVase,
    CollectibleGoblet,
    UtilityBackpack
}
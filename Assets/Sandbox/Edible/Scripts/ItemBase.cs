using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemBase : MonoBehaviour, IInteractable
{
    public ItemData ItemData;
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
        Debug.Log("Chujek");

        if (!CanInteract || IsBlocked)
        {
            Debug.Log("Chuj");
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

        if (playerBase.PickUp(ItemData))
        {
            Debug.Log("Chhuj");
            Destroy(gameObject);
        }
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
}

[Serializable]
public class ItemData
{
    [Header("Item Attributes")] 
    public ItemType ItemType;
    public string ItemName;
    public string ItemDescription;
    public int ItemSize;
    public float ItemValue;
    public Sprite ItemIcon;
    public GameObject ItemPrefab;
}

public enum ItemType
{
    Debug,
    CollectibleVase,
    CollectibleGoblet,
    UtilityBackpack
}
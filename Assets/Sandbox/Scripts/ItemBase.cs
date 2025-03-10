using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Script.ScriptableObjects;
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

    public bool IsUsed { get; set; } = false;

    [SerializeField] private string blockedMessage;
    private IStatChangeable statChangeableImplementation;

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

        if (playerBase.PickUp(ItemData))
        {
            InteractEvent.Raise(this, null);
            ItemData.AddModifier(playerBase.PlayerStatsController);
            IsUsed = true;
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

public interface IStatChangeable
{
    public void AddModifier(SOPlayerStatsController statToControll);
    public void RemoveModifier(SOPlayerStatsController statToControll);
    public List<StatParameters> StatsToChange { get; set; }
}

[Serializable]
public struct StatParameters
{
    public E_ModifiersType ModifierType;
    public float Additive;
    public float Multiplicative;
}

[Serializable]
public class ItemData : IStatChangeable
{
    [Header("Item Attributes")] public ItemType ItemType;
    public string ItemName;
    public string ItemDescription;
    public int ItemSize;
    public float ItemValue;
    public Sprite ItemIcon;
    public GameObject ItemPrefab;

    public List<StatParameters> StatsToChange
    {
        get => statToChange;
        set => statToChange = value;
    }

    public List<StatParameters> statToChange;

    public void AddModifier(SOPlayerStatsController statToControll)
    {
        if(StatsToChange == null)
            return;
        
        foreach (var stat in StatsToChange)
        {
            statToControll.ChangeModifier(stat.Additive, stat.Multiplicative, stat.ModifierType);
        }
    }
    
    public void RemoveModifier(SOPlayerStatsController statToControll)
    {
        if(StatsToChange == null)
            return;
        
        foreach (var stat in StatsToChange)
        {
            statToControll.ChangeModifier(-stat.Additive, -stat.Multiplicative, stat.ModifierType);
        }
    }
}

public enum ItemType
{
    Debug,
    CollectibleVase,
    CollectibleGoblet,
    UtilityBackpack,
    CollectibleBox,


    //Useable
    Key,
    Axe,
    SteelShoes,
    FastShoes,
    Forklift
}
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

    public virtual void Interact(Transform player)
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
            GetComponent<PickupSounds>().PlayPickupSound();
            InteractEvent.Raise(this, null);
            ItemData.AddModifier(playerBase.PlayerStatsController);
            if (ItemData.StatsToChange.Any(x => x.ModifierType == E_ModifiersType.Inventory))
            {
                playerBase.ResetInventory();
            }
            if (ItemData.StatsToChange.Any(x => x.ModifierType == E_ModifiersType.Stamina))
            {
                playerBase.ResetStamina();
            }
            if (ItemData.StatsToChange.Any(x => x.ModifierType == E_ModifiersType.Fuel))
            {
                playerBase.ResetFuel();
            }
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
        var textToShow = InputManager.Instance.CompereTextWithInput("Interaction", interactMessage);
        ShowUIEvent.Raise(this, (true, textToShow, false));   
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
    
    public bool ChangedStats { get; set; }
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
    public bool ChangedStats { get; set; }
    
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
    public int CollectibleId;
    public bool ProceedToDungeon;

    public string ReturnData()
    {
        string returnData = "";
        returnData += ItemName + ", ";
        returnData += ItemDescription + ", ";
        returnData += ItemSize + ", ";
        returnData += ItemValue + ", ";
        returnData += ItemIcon.name + ", ";
        returnData += ItemPrefab.gameObject.name+ ", ";
        return returnData;
    }
}

public enum ItemType
{
    //DEBUG 0 - 9
    Debug = 0,
    
    //Collectible 10 - 99
    CollectibleVase = 10,
    CollectibleGoblet = 11,
    CollectibleBook = 12,
    
    //Usable 100 - 200
    Backpack = 100, 
    Key = 101,
    Hammer = 102,
    SteelShoes = 103,
    FastShoes = 104,
    Forklift = 105,
    Potion = 106, 
    Unused1 = 107,
    Unused2 = 108,
    Unused3 = 109,
    Unused4 = 110,
    Unused5 = 111,
    Unused6 = 112,
    MoneyBag = 113
}

public static class ItemTypeHelper
{
    public static List<ItemType> GetCollectibles()
    {
        return Enum.GetValues(typeof(ItemType))
            .Cast<ItemType>()
            .Where(i => (int)i >= 10 && (int)i < 100)
            .ToList();
    }
}
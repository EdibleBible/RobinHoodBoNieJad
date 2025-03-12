using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using NUnit.Framework;
using Script.ScriptableObjects;
using UnityEngine;

public class SlowingGlueController : MonoBehaviour, IInteractable, IStatChangeable
{
    public bool ChangedStats { get; set; }
    public List<StatParameters> StatsToChange
    {
        get => statToChange;
        set => statToChange = value;
    }

    public List<StatParameters> statToChange;

    [Header("FMOD")] [SerializeField] private EventReference pressSoundEvent;
    [SerializeField] private EventReference releaseSoundEvent;
    [SerializeField] private Transform soundSource;
    private EventInstance pressSoundInstance;
    private EventInstance releaseSoundInstance;


    public void StartSlowingGlue(SOPlayerStatsController statToControll)
    {
        AddModifier(statToControll);
    }

    public void StopSlowingGlue(SOPlayerStatsController statToControll)
    {
        RemoveModifier(statToControll);
    }

    public void AddModifier(SOPlayerStatsController statToControll)
    {
        if (StatsToChange == null)
            return;

        if (!ChangedStats)
        {
            foreach (var stat in StatsToChange)
            {
                statToControll.ChangeModifier(stat.Additive, stat.Multiplicative, stat.ModifierType);
            }
            ChangedStats = true;
        }
    }

    public void RemoveModifier(SOPlayerStatsController statToControll)
    {
        if (StatsToChange == null)
            return;

        if (ChangedStats)
        {
            foreach (var stat in StatsToChange)
            {
                statToControll.ChangeModifier(-stat.Additive, -stat.Multiplicative, stat.ModifierType);
            }
            ChangedStats = false;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent<PlayerBase>(out PlayerBase playerBase))
        {
            return;
        }
        
        if (!playerBase.PlayerInventory.ItemsInInventory.Any(x => x.ItemType == ItemType.FastShoes))
            StartSlowingGlue(playerBase.PlayerStatsController);
        else
        {
            if (ChangedStats)
            {
                RemoveModifier(playerBase.PlayerStatsController);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        
        if (other.TryGetComponent<PlayerBase>(out PlayerBase playerBase))
        {
            StopSlowingGlue(playerBase.PlayerStatsController);
        }
        else
        {
            Debug.Log(other.gameObject.name);
        }
    }

    private void PlayGlueSound()
    {
    }

    public void Interact(Transform player)
    {
        //empty
    }

    public void ShowUI()
    {
        //empty
    }

    public void HideUI()
    {
        //empty
    }

    [HideInInspector] public GameEvent ShowUIEvent { get; set; }
    [HideInInspector] public GameEvent InteractEvent { get; set; }
    [HideInInspector] public bool CanInteract { get; set; }
    [HideInInspector] public bool IsBlocked { get; set; }
    [HideInInspector] public bool TwoSideInteraction { get; set; }
    [HideInInspector] public string InteractMessage { get; set; }
    [HideInInspector] public string BlockedMessage { get; set; }
    [HideInInspector] public bool IsUsed { get; set; }
}
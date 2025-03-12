using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Script.ScriptableObjects;
using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable, IInteractableStop, IStatChangeable
{
    public Rigidbody rb;

    public GameEvent showUIEvent;
    public GameEvent interactEvent;

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

    public bool CanInteract { get; set; } = true;
    public bool IsBlocked { get; set; } = false;
    public bool TwoSideInteraction { get; set; } = true;

    public string interactMessage;
    public string blockMessage;

    public string InteractMessage
    {
        get => interactMessage;
        set => interactMessage = value;
    }

    public string BlockedMessage
    {
        get => blockMessage;
        set => blockMessage = value;
    }

    public bool IsUsed { get; set; } = false;
    public bool IsInteracting { get; set; }

    public Vector3 BaseSize;
    public Vector3 InteractSize;
    public Vector3 PickupOffset;

    public List<StatParameters> statsToChange;

    public List<StatParameters> StatsToChange
    {
        get => statsToChange;
        set => statsToChange = value;
    }

    public bool ChangedStats { get; set; }

    [SerializeField] private SOPlayerStatsController statsControll;

    private PlayerBase playerBase;
    private PlayerInteractionController playerController;
    private Coroutine forkliftCheckCoroutine;

    private void Awake()
    {
        BaseSize = transform.localScale;
    }

    public void StopInteracting()
    {
        if (!IsUsed && !IsInteracting)
            return;

        Drop();
        HideUI();
        ShowUI();
    }

    public void Interact(Transform player)
    {
        if (IsUsed && IsInteracting)
            return;

        Pickup(player);
        HideUI();
        ShowStopUI();
    }

    public void Pickup(Transform player)
    {
        if (playerController == null)
            playerController = player.GetComponent<PlayerInteractionController>();
        if (playerBase == null)
            playerBase = playerController.GetComponent<PlayerBase>();

        Transform holdPosition = playerController.GetHoldPosition();
        rb.isKinematic = true;
        transform.SetParent(holdPosition);
        transform.localPosition = Vector3.zero + PickupOffset;
        transform.localRotation = Quaternion.identity;
        ChangeSize(InteractSize);
        if (!playerBase.PlayerInventory.ItemsInInventory.Any(x => x.ItemType == ItemType.Forklift))
            AddModifier(statsControll);
        IsUsed = true;
        IsInteracting = true;
        forkliftCheckCoroutine = StartCoroutine(CheckForForklift());
    }

    public void Drop()
    {
        rb.isKinematic = false;
        transform.SetParent(null);
        ChangeSize(BaseSize);
        RemoveModifier(statsControll);
        IsUsed = false;
        IsInteracting = false;
        if (forkliftCheckCoroutine != null)
        {
            StopCoroutine(forkliftCheckCoroutine);
            forkliftCheckCoroutine = null;
        }
    }

    private IEnumerator CheckForForklift()
    {
        while (IsUsed)
        {
            bool hasForklift = playerBase.PlayerInventory.ItemsInInventory.Any(x => x.ItemType == ItemType.Forklift);
            Debug.Log("Check: " + hasForklift);

            
            if (hasForklift && ChangedStats)
            {
                RemoveModifier(statsControll);
            }
            else if (!hasForklift && !ChangedStats)
            {
                AddModifier(statsControll);
            }

            yield return new WaitForSeconds(0.5f); // Sprawdzanie co pół sekundy
        }
    }

    public void ChangeSize(Vector3 newSize)
    {
        transform.localScale = newSize;
    }

    public void ShowUI()
    {
        if (!CanInteract)
            return;
        ShowUIEvent.Raise(this, (true, "Pickup", false));
    }

    public void ShowStopUI()
    {
        if (!CanInteract)
            return;

        ShowUIEvent.Raise(this, (true, "Drop", false));
    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this, (false, "", false));
    }

    public void AddModifier(SOPlayerStatsController statToControll)
    {
        if (ChangedStats)
            return;
        if (StatsToChange == null)
            return;

        foreach (var stat in StatsToChange)
        {
            statToControll.ChangeModifier(stat.Additive, stat.Multiplicative, stat.ModifierType);
        }
        
        ChangedStats = true;
    }

    public void RemoveModifier(SOPlayerStatsController statToControll)
    {
        if (!ChangedStats)
            return;
        if (StatsToChange == null)
            return;

        foreach (var stat in StatsToChange)
        {
            statToControll.ChangeModifier(-stat.Additive, -stat.Multiplicative, stat.ModifierType);
        }
        
        ChangedStats = false;
    }
}

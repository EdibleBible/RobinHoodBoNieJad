using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class BreakingWallController : MonoBehaviour, IInteractable
{
    public string InteractMessage
    {
        get => interactionMessage;
        set => interactionMessage = value;
    }

    public string interactionMessage;

    public string BlockedMessage
    {
        get => blockedMessage;
        set => blockedMessage = value;
    }

    public string blockedMessage;
    public bool IsUsed { get; set; }

    [Header("FMOD")] public EventReference doorsEvent;
    public Transform soundSource;
    public EventInstance BreakSound;

    [Header("Events")] public GameEvent showUIEvent;
    public GameEvent interactEvent;

    public GameObject doorsModel;
    public Collider doorCollider;
    public Animator doorAnimator;

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

    public void Interact(Transform player)
    {
        if (IsUsed)
        {
            return;
        }

        PlayerBase playerBase = player.GetComponent<PlayerBase>();
        if (playerBase.PlayerInventory.ItemsInInventory.Any(x => x.ItemType == ItemType.Hammer))
        {
            Debug.Log("used ");

            PlayWallSound();
            PlayWallAnimation();

            IsUsed = true;
            CanInteract = false;
        }
        else
        {
            ShowUIEvent.Raise(this, (true, BlockedMessage, true));
            Debug.Log("cant use hammer");
        }
    }

    public virtual void ShowUI()
    {
        ShowUIEvent.Raise(this, (true, InteractMessage, false));
    }

    public virtual void HideUI()
    {
        ShowUIEvent.Raise(this, (false, "", false));
    }

    public void PlayWallSound(float volume = 0.2f)
    {
    }

    public void PlayWallAnimation()
    {
        doorAnimator.SetTrigger("Interact");
    }

    public void RemoveDoors()
    {
        if (doorsModel != null)
            doorsModel.gameObject.SetActive(false);
        
        doorCollider.gameObject.SetActive(false);
    }

    [HideInInspector] public bool CanInteract { get; set; } = true;
    [HideInInspector] public bool IsBlocked { get; set; } = false;
    [HideInInspector] public bool TwoSideInteraction { get; set; } = false;
}
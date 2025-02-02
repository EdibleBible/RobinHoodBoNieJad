using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class LeverController : MonoBehaviour, IInteractable
{
    [Header("Reference")]
    [SerializeField] private GameObject gameobjectToInteract; 
    private IInteractable objectToInteract;
    [SerializeField] private Animator animator;
    
    [Header("Bools")]
    public bool TwoSideInteraction
    {
        get => twoSideInteraction;
        set => twoSideInteraction = value;
    }
    
    public bool twoSideInteraction;
    public bool CanInteract { get; set; } = true;
    public bool IsBlocked { get; set; }

    [Header("FMOD")] 
    [SerializeField] private EventReference doorsEvent;
    [SerializeField] private Transform soundSource;
    private EventInstance doorSoundInstance;
    
    [Header("Events")] 
    [SerializeField] private GameEvent showUIEvent;
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

    private void Awake()
    {
        if (gameobjectToInteract.TryGetComponent<IInteractable>(out IInteractable iInteractable))
        {
            objectToInteract = iInteractable;
            objectToInteract.IsBlocked = true;
        }
    }

    public void Interact(Transform player)
    {
        if (IsBlocked)
        {
            ShowUIEvent.Raise(this, (true, BlockedMessage, true));
            return;
        }

        if (!CanInteract)
        {
            return;
        }
        
        InteractEvent.Raise(this, null);
        
        if (!TwoSideInteraction)
        {
            HideUI();
            objectToInteract.IsBlocked = false;
            objectToInteract.TwoSideInteraction = false;
            CanInteract = false;
        }

        objectToInteract.IsBlocked = false;
        objectToInteract.Interact(player);
        animator.SetTrigger("Interact");
        

    }
    public void ShowUI()
    {
        ShowUIEvent.Raise(this, (true, InteractMessage,false));
    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this, (false, "",false));
    }
}

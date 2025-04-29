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
    [SerializeField] private EventReference leverDownEvent; // Dodano event dla d�wigni
    [SerializeField] private Transform soundSource;
    private EventInstance doorSoundInstance;
    private EventInstance leverSoundInstance;

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

    public bool IsUsed { get; set; }
    [SerializeField] private string blockedMessage;

    private void Awake()
    {
        if (gameobjectToInteract == null)
            return;

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
            if (objectToInteract != null)
            {
                objectToInteract.IsBlocked = false;
                objectToInteract.TwoSideInteraction = false;
            }
            CanInteract = false;
        }

        if (objectToInteract != null)
        {
            objectToInteract.IsBlocked = false;
            objectToInteract.Interact(player);
        }

        animator.SetTrigger("Interact");
        IsUsed = !IsUsed;
        PlayLeverSound(); // Odtwarzanie d�wi�ku d�wigni
    }

    private void PlayLeverSound()
    {
        leverSoundInstance = RuntimeManager.CreateInstance(leverDownEvent);
        if (soundSource != null)
        {
            RuntimeManager.AttachInstanceToGameObject(leverSoundInstance, soundSource);
        }
        //leverSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Vector3.zero));
        leverSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
        leverSoundInstance.setVolume(0.3f);
        leverSoundInstance.start();
        leverSoundInstance.release();
    }

    public void ShowUI()
    {
        if (IsBlocked || !CanInteract)
            return;

        var textToShow = InputManager.Instance.CompereTextWithInput("Interaction", interactMessage);
        ShowUIEvent.Raise(this, (true, textToShow, false));
        
    }

    public void HideUI()
    {
        if (IsBlocked || !CanInteract)
            return;

        ShowUIEvent.Raise(this, (false, "", false));
    }
}
using System;
using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable
{
    [Header("Bools")]
    public bool TwoSideInteraction
    {
        get => twoSideInteraction;
        set => twoSideInteraction = value;
    }

    public bool twoSideInteraction;
    public bool CanInteract { get; set; } = true;
    public bool IsBlocked { get; set; }

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

    public string AlternativeInteractMessage
    {
        get => alternativeInteractMessage;
        set => alternativeInteractMessage = value;
    }

    [SerializeField] private string alternativeInteractMessage;
    

    [SerializeField] private string blockedMessage;

    public string BlockedMessage
    {
        get => blockedMessage;
        set => blockedMessage = value;
    }
    public bool IsUsed { get; set; }


    [Header("IK Animation")] [SerializeField]
    private Transform LeftHandIKPosition;

    [SerializeField] private Transform RightHandIKPosition;

    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private Vector3 pickUpOffset;
    [SerializeField] private Vector3 baseScale;
    [SerializeField] private Vector3 pickupScale;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void Interact(Transform player)
    {
        if (IsBlocked)
            return;

        if (!IsUsed)
        {
            PlayerInteractionController interactionController = player.GetComponent<PlayerInteractionController>();
            transform.SetParent(interactionController.GetHoldPosition());
            transform.localPosition = Vector3.zero + pickUpOffset;
            transform.localRotation = Quaternion.identity;
            transform.localScale = pickupScale;
            rb.isKinematic = true;
            IsUsed = true;
            HideUI();
            ShowUI();
        }
        else
        {
            rb.isKinematic = false;
            transform.SetParent(null);
            IsUsed = false;
            transform.localScale = baseScale;
            HideUI();
            ShowUI();
        }
    }

    public void ShowUI()
    {
        if (IsBlocked)
            return;

        if (IsUsed)
            ShowUIEvent.Raise(this, (true, InteractMessage, false));
        else
        {
            ShowUIEvent.Raise(this, (true, alternativeInteractMessage, false));

        }
    }

    public void HideUI()
    {
        if (IsBlocked)
            return;

        ShowUIEvent.Raise(this, (false, "", false));
    }
}
using System;
using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable , IInteractableStop
{
    public Rigidbody rb;
    
    public GameEvent showUIEvent;
    public GameEvent interactEvent;
    
    public GameEvent ShowUIEvent { get => showUIEvent; set => showUIEvent = value; }
    public GameEvent InteractEvent { get => interactEvent; set => interactEvent = value; }
    public bool CanInteract { get; set; } = true;
    public bool IsBlocked { get; set; } = false;
    public bool TwoSideInteraction { get; set; } = true;

    public string interactMessage;
    public string blockMessage;
    public string InteractMessage { get => interactMessage; set => interactMessage = value; }
    public string BlockedMessage { get => blockMessage; set => blockMessage = value; }
    public bool IsUsed { get; set; } = false;
    public bool IsInteracting { get; set; }

    public Vector3 BaseSize;
    public Vector3 InteractSize;
    public Vector3 PickupOffset;

    private void Awake()
    {
        BaseSize = transform.localScale;
    }

    public void StopInteracting()
    {
        if(!IsUsed && !IsInteracting)
            return;
        
        Drop();
        HideUI();
        ShowUI();
    }
    
    public void Interact(Transform player)
    {
        if(IsUsed && IsInteracting)
            return;
        
        Pickup(player);
        HideUI();
        ShowStopUI();
    }

    public void Pickup(Transform player)
    {
        PlayerInteractionController playerBase = player.GetComponent<PlayerInteractionController>();
        Transform holdPosition = playerBase.GetHoldPosition();
        rb.isKinematic = true;
        transform.SetParent(holdPosition);
        transform.localPosition = Vector3.zero + PickupOffset;
        transform.localRotation = Quaternion.identity;
        ChangeSize(InteractSize);

        
        IsUsed = true;
        IsInteracting = true;
    }

    public void Drop()
    {
        rb.isKinematic = false;
        transform.SetParent(null);
        ChangeSize(BaseSize);

        
        IsUsed = false;
        IsInteracting = false;        
    }

    public void ChangeSize(Vector3 newSize)
    {
        transform.localScale = newSize;
    }

    public void ShowUI()
    {
        if(!CanInteract)
            return;
        ShowUIEvent.Raise(this,(true, "Pickup", false));

    }
    
    public void ShowStopUI()
    {
        if(!CanInteract)
            return;
        
        ShowUIEvent.Raise(this,(true, "Drop", false));

    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this,(false, "", false));
    }
}
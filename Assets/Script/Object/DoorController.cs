using System;
using UnityEngine;
using DG.Tweening;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject doorPivot;

    public bool CanInteract
    {
        get => canInteract;
        set => canInteract = value;
    }

    public bool canInteract = true;
    [Header("Open")] [SerializeField] private Vector3 OpenDoorPosition;
    [SerializeField] private float OpenTime;
    [SerializeField] private AnimationCurve OpenCurve;
    [Header("Close")] [SerializeField] private Vector3 CloseDoorPosition;
    [SerializeField] private float CloseTime;
    [SerializeField] private AnimationCurve CloseCurve;

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


    [Header("Debug")] [SerializeField] private bool isDebug;
    [SerializeField] private KeyCode debugKey = KeyCode.C;
    
    private bool isOpen = false;
    private Tween isDoorOpenTween;
    public bool IsBlocked;
    public bool TwoSideInteraction;

    public void Interact()
    {
        if (IsBlocked)
            return;

        if (!isOpen && isDoorOpenTween == null && CanInteract)
        {
            InteractEvent.Raise(this, null);
            isDoorOpenTween = doorPivot.transform.DOLocalRotate(OpenDoorPosition, OpenTime).SetEase(OpenCurve)
                .OnComplete(() =>
                {
                    isDoorOpenTween = null;
                    isOpen = true;
                    if (!TwoSideInteraction)
                    {
                        CanInteract = false;
                    }
                });
        }
        else if (isOpen && isDoorOpenTween == null && CanInteract)
        {
            InteractEvent.Raise(this, null);
            isDoorOpenTween = doorPivot.transform.DOLocalRotate(CloseDoorPosition, CloseTime).SetEase(CloseCurve)
                .OnComplete(() =>
                {
                    isDoorOpenTween = null;
                    isOpen = false;
                });
        }
    }

    public void ShowUI()
    {
        ShowUIEvent.Raise(this, (true, InteractMessage));
    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this, (false, ""));
    }
}
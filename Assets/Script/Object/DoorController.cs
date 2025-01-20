using System;
using UnityEngine;
using DG.Tweening;

public class DoorController : MonoBehaviour, IInteractable
{

    public bool CanInteract
    {
        get => canInteract;
        set => canInteract = value;
    }

    public bool canInteract = true;

    [SerializeField] private float openTime;
    [SerializeField] private AnimationCurve openCurve;
    
    [SerializeField] private float closeTime;
    [SerializeField] private AnimationCurve closeCurve;
    
    
    [Header("LeftDoor")]
    [SerializeField] private GameObject doorPivotLeft;
    [SerializeField] private Vector3 leftDoorClosePosition;
    [SerializeField] private Vector3 leftDoorOpenPosition;
    
    [Header("RightDoor")]
    [SerializeField] private GameObject doorPivotRight;
    [SerializeField] private Vector3 rightDoorClosePosition;
    [SerializeField] private Vector3 rightDoorOpenPosition;

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

            // Tworzymy tweena dla lewych drzwi
            Tween leftDoorTween = doorPivotLeft.transform.DOLocalRotate(leftDoorOpenPosition, openTime).SetEase(openCurve);

            // Tworzymy tweena dla prawych drzwi
            Tween rightDoorTween = doorPivotRight.transform.DOLocalRotate(rightDoorOpenPosition, openTime).SetEase(openCurve);

            // Łączymy tweens w Sequence
            isDoorOpenTween = DOTween.Sequence()
                .Join(leftDoorTween)
                .Join(rightDoorTween)
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

            // Tworzymy tweena dla lewych drzwi
            Tween leftDoorTween = doorPivotLeft.transform.DOLocalRotate(leftDoorClosePosition, closeTime).SetEase(closeCurve);

            // Tworzymy tweena dla prawych drzwi
            Tween rightDoorTween = doorPivotRight.transform.DOLocalRotate(rightDoorClosePosition, closeTime).SetEase(closeCurve);

            // Łączymy tweens w Sequence
            isDoorOpenTween = DOTween.Sequence()
                .Join(leftDoorTween)
                .Join(rightDoorTween)
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
using System;
using UnityEngine;
using DG.Tweening;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject doorPivot;
    private bool isOpen = false;
    private Tween isDoorOpenTween;


    [Header("Open")] [SerializeField] private Vector3 OpenDoorPosition;
    [SerializeField] private float OpenTime;
    [SerializeField] private AnimationCurve OpenCurve;
    [Header("Close")] [SerializeField] private Vector3 CloseDoorPosition;
    [SerializeField] private float CloseTime;
    [SerializeField] private AnimationCurve CloseCurve;

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

    [Header("Settings")]
    [HideInInspector] public bool CanInteract { get; set; }

    
    [Header("Callbacks")] 
    public string InteractMessage
    {
        get => interactMessage;
        set => interactMessage = value;
    }
    [SerializeField] private string interactMessage; 


    [Header("Debug")] 
    [SerializeField] private bool isDebug;
    [SerializeField] private KeyCode debugKey = KeyCode.C;
    

    private void Update()
    {
        if (isDebug && Input.GetKeyDown(debugKey))
        {
            CanInteract = true;
            Interact();
            CanInteract = false;
        }
    }

    public void Interact()
    {
        Debug.Log("Interact");
        if (!isOpen && isDoorOpenTween == null && CanInteract)
        {
            Debug.Log("Open");
            InteractEvent.Raise(this, null);
            isDoorOpenTween = doorPivot.transform.DOLocalRotate(OpenDoorPosition, OpenTime).SetEase(OpenCurve)
                .OnComplete(() =>
                {
                    isDoorOpenTween = null;
                    isOpen = true;
                });
        }
        else if (isOpen && isDoorOpenTween == null && CanInteract)
        {
            Debug.Log("Close");
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
        ShowUIEvent.Raise(this, (false, InteractMessage));
    }
}
using System;
using UnityEngine;

public class LockPickDoorController : DoorController, ILockPick
{
    public bool IsInteracting { get; set; }
    public bool IsLocked
    {
        get => isLocked;
        set => isLocked = value;
    }
    public GameObject LockPickPrefab
    {
        get => lockPickPrefab;
        set => lockPickPrefab = value;
    }
    public Transform LockPickCameraTransform
    {
        get => lockPickCameraTransform;
        set => lockPickCameraTransform = value;
    }
    public Vector3 LockPickPositionOffset
    {
        get => lockPickRotationOffset;
        set => lockPickRotationOffset = value;
    }
    public Vector3 LockPickScale
    {
        get => lockPickScale;
        set => lockPickScale = value;
    }
    public KeyCode CancelLockPickingKeyCode
    {
        get => cancelLockPickingKeyCode;
        set => cancelLockPickingKeyCode = value;
    }

    public bool IsLockPicking { get; set; }
    public GameObject SpawnedObject { get; set; }
    public LockPick spawnedLockPick { get; set; }
    public PlayerStateMachineController StateMachineController { get; set; }
    
    public bool isLocked;
    public KeyCode cancelLockPickingKeyCode; 
    
    public GameObject lockPickPrefab;
    public Transform lockPickCameraTransform;
    public Vector3 lockPickRotationOffset;
    public Vector3 lockPickScale
        ;
    

    [SerializeField] private string ChnagedInteractMessage;

    private void Awake()
    {
        if (!IsLocked)
        {
            InteractMessage = ChnagedInteractMessage;
        }
    }

    public override void Interact(Transform player)
    {
        if (StateMachineController == null)
        {
            StateMachineController = player.GetComponent<PlayerStateMachineController>();
        }
        
        if (IsLocked)
        {
            if(IsLockPicking)
                return;
            
            PlayerBase playerBase = player.GetComponent<PlayerBase>();
            ItemData currSelectedItem = playerBase.CurrSelectedItem;

            if (currSelectedItem != null)
            {
                if (currSelectedItem.ItemType == ItemType.Key)
                {
                    playerBase.RemoveItemFromInventory(currSelectedItem);
                    IsLocked = false;
                    base.Interact(player);
                    InteractMessage = ChnagedInteractMessage;
                    HideUI();
                    ShowUI();
                }
                else
                {
                    Lockipick(playerBase);
                }
            }
            else if (currSelectedItem == null || currSelectedItem.ItemType != ItemType.Key)
            {
                Lockipick(playerBase);
            }
        }
        else
        {
            base.Interact(player);
        }
    }

    private void Lockipick(PlayerBase playerBase)
    {
        Vector3 spawnPosition = playerBase.camera.transform.position;
        SpawnedObject = Instantiate(lockPickPrefab, spawnPosition, playerBase.camera.transform.rotation, playerBase.camera.transform);
        SpawnedObject.transform.localScale = lockPickScale;
        SpawnedObject.transform.localPosition += LockPickPositionOffset;
        spawnedLockPick = SpawnedObject.GetComponentInChildren<LockPick>();
        spawnedLockPick.SetUpCamera(playerBase.camera);
        spawnedLockPick.SetObjectToLockPick(this);
        StateMachineController.SeePlayerInteracting(true);
        IsLockPicking = true;
        HideUI();
        ShowUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(CancelLockPickingKeyCode) && IsLockPicking)
        {
            StopInteracting();
        }
    }


    public void StopInteracting()
    {
        if(StateMachineController == null)
            return;
        
        Destroy(SpawnedObject);
        StateMachineController.SeePlayerInteracting(false);
        IsLockPicking = false;
    }

    public void UnlockLock()
    {
        IsLocked = false;
        InteractMessage = ChnagedInteractMessage;
        HideUI();
        ShowUI();
    }

    public override void ShowUI()
    {
        if (!IsLockPicking)
            base.ShowUI();
    }
}

public interface ILockPick
{
    public bool IsLockPicking { get; set; }
    public bool IsLocked { get; set; }
    public GameObject LockPickPrefab { get; set; }
    public Transform LockPickCameraTransform { get; set; }

    public GameObject SpawnedObject { get; set; }
    public LockPick spawnedLockPick { get; set; }
    public PlayerStateMachineController StateMachineController { get; set; }
    public KeyCode CancelLockPickingKeyCode { get; set; }
    public Vector3 LockPickPositionOffset { get; set; }
    public Vector3 LockPickScale { get; set; }

    public void StopInteracting();
    public void UnlockLock();
}
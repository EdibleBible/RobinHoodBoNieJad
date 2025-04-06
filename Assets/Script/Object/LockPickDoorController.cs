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
    public Vector3 lockPickScale;
    public float lockPickSpawnDistance;


    [SerializeField]
    private string ChnagedInteractMessage;

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
            if (IsLockPicking)
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
                    Lockpick(playerBase);
                }
            }
            else if (currSelectedItem == null || currSelectedItem.ItemType != ItemType.Key)
            {
                Lockpick(playerBase);
            }
        }
        else
        {
            base.Interact(player);
        }
    }

    private void Lockpick(PlayerBase playerBase)
    {
        // Pobranie pozycji kamery i kierunku, w którym patrzy gracz
        Transform cameraTransform = playerBase.camera.transform;
        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * lockPickSpawnDistance;

        // Instancja lockpicka i poprawne ustawienie transformacji
        SpawnedObject = Instantiate(lockPickPrefab, spawnPosition, cameraTransform.rotation);
        SpawnedObject.transform.localScale = lockPickScale;

        // Ustawienie offsetu względem pozycji gracza (opcjonalne, jeśli wymagane)
        SpawnedObject.transform.position += cameraTransform.right * LockPickPositionOffset.x;
        SpawnedObject.transform.position += cameraTransform.up * LockPickPositionOffset.y;
        SpawnedObject.transform.position += cameraTransform.forward * LockPickPositionOffset.z;

        // Pobranie komponentu LockPick i ustawienie jego właściwości
        spawnedLockPick = SpawnedObject.GetComponentInChildren<LockPick>();
        spawnedLockPick.SetUpCamera(playerBase.camera);
        spawnedLockPick.SetObjectToLockPick(this);

        // Aktualizacja stanu gry
        StateMachineController.SeePlayerInteracting(true);
        IsLockPicking = true;

        // Aktualizacja interfejsu użytkownika
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
        if (StateMachineController == null)
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
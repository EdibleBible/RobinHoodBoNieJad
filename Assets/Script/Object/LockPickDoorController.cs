using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class LockPickDoorController : DoorController, ILockPick, IInteractableStop
{
    [SerializeField] private EventReference lockpickSuccessSound;
    [SerializeField] private EventReference lockpickManipulationSound;

    [SerializeField] private EventReference lockpickRotationSound;

    private EventInstance rotationSoundInstance;
    private bool isRotationSoundPlaying = false;


    private EventInstance manipulationSoundInstance;
    private bool isManipulatingSoundPlaying = false;
    private float mouseMoveThreshold = 0.05f;


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

    public bool IsLockPicking { get; set; }
    public GameObject SpawnedObject { get; set; }
    public LockPick spawnedLockPick { get; set; }
    public PlayerStateMachineController StateMachineController { get; set; }

    public bool isLocked;

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

        // Start sound instance for manipulation
        manipulationSoundInstance = RuntimeManager.CreateInstance(lockpickManipulationSound);
        RuntimeManager.AttachInstanceToGameObject(manipulationSoundInstance, transform, GetComponent<Rigidbody>());
        manipulationSoundInstance.start();
        manipulationSoundInstance.setPaused(true); // Start paused, unpause when moving

        // Start sound instance for rotation
        rotationSoundInstance = RuntimeManager.CreateInstance(lockpickRotationSound);
        RuntimeManager.AttachInstanceToGameObject(rotationSoundInstance, transform, GetComponent<Rigidbody>());
        rotationSoundInstance.start();
        rotationSoundInstance.setPaused(true); // Start paused, unpause when holding LPM

        // Aktualizacja interfejsu użytkownika
        HideUI();
        ShowUI();
    }

    public void ShowStopUI()
    {
    }


    private float currentPadlockOpeningValue = 0f;
    [SerializeField] private float padlockSmoothingSpeed = 5f;

    private void Update()
    {
        if (IsLockPicking)
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            // Obsługa dźwięku manipulacji (ruchy)
            if (mouseDelta.magnitude > mouseMoveThreshold)
            {
                if (!isManipulatingSoundPlaying)
                {
                    manipulationSoundInstance.setPaused(false);
                    isManipulatingSoundPlaying = true;
                }
            }
            else
            {
                if (isManipulatingSoundPlaying)
                {
                    manipulationSoundInstance.setPaused(true);
                    isManipulatingSoundPlaying = false;
                }
            }
        }

        // Obsługa dźwięku obracania zamka (PadlockOpening)
        float targetPadlockValue = Input.GetMouseButton(0) ? 1f : 0f;
        currentPadlockOpeningValue = Mathf.Lerp(currentPadlockOpeningValue, targetPadlockValue, Time.deltaTime * padlockSmoothingSpeed);
        manipulationSoundInstance.setParameterByName("PadlockOpening", currentPadlockOpeningValue);

        // Obsługa osobnego dźwięku obracania zamka (loop, LPM)
        if (Input.GetMouseButton(0))
        {
            if (!isRotationSoundPlaying)
            {
                rotationSoundInstance.setPaused(false);
                isRotationSoundPlaying = true;
            }
        }
        else
        {
            if (isRotationSoundPlaying)
            {
                rotationSoundInstance.setPaused(true);
                isRotationSoundPlaying = false;
            }
        }

    }




    public void StopInteracting()
    {
        if (StateMachineController == null)
            return;

        Destroy(SpawnedObject);
        StateMachineController.SeePlayerInteracting(false);
        IsLockPicking = false;

        if (manipulationSoundInstance.isValid())
        {
            manipulationSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            manipulationSoundInstance.release();
        }

        if (rotationSoundInstance.isValid())
        {
            rotationSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            rotationSoundInstance.release();
        }

    }

    public void UnlockLock()
    {
        RuntimeManager.PlayOneShot(lockpickSuccessSound, transform.position);
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
    public Vector3 LockPickPositionOffset { get; set; }
    public Vector3 LockPickScale { get; set; }

    public void StopInteracting();
    public void UnlockLock();
}
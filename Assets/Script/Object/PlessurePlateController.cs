using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PressurePlateController : MonoBehaviour, IInteractable
{
    [Header("Reference")] [SerializeField] private GameObject gameobjectToInteract;
    private IInteractable objectToInteract;
    [SerializeField] private Transform plateTransform;

    [Header("Settings")] [SerializeField] private float pressDepth = 0.1f;
    [SerializeField] private float pressDuration = 0.2f;
    [SerializeField] private float releaseDuration = 0.2f;

    [Header("Bools")]
    public bool TwoSideInteraction
    {
        get => twoSidesInteraction;
        set => twoSidesInteraction = value;
    }

    public bool twoSidesInteraction;
    public bool CanInteract { get; set; } = true;
    public bool IsBlocked { get; set; }
    public bool IsUsed { get; set; }

    [Header("FMOD")] [SerializeField] private EventReference pressSoundEvent;
    [SerializeField] private EventReference releaseSoundEvent;
    [SerializeField] private Transform soundSource;
    private EventInstance pressSoundInstance;
    private EventInstance releaseSoundInstance;

    [Header("Events")] [SerializeField] private GameEvent interactEvent;
    public GameEvent ShowUIEvent { get; set; }

    public GameEvent InteractEvent
    {
        get => interactEvent;
        set => interactEvent = value;
    }

    [Header("Callbacks")] public string InteractMessage { get; set; }
    public string BlockedMessage { get; set; }

    private HashSet<GameObject> pressingObjects = new();
    private Coroutine checkCoroutine;
    private Vector3 initialPosition;

    private void Awake()
    {
        if (gameobjectToInteract != null &&
            gameobjectToInteract.TryGetComponent<IInteractable>(out IInteractable iInteractable))
        {
            objectToInteract = iInteractable;
            objectToInteract.TwoSideInteraction = TwoSideInteraction;
            objectToInteract.IsBlocked = true;
        }

        if (plateTransform == null)
        {
            plateTransform = transform;
        }

        initialPosition = plateTransform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsBlocked || !CanInteract || !IsValidObject(other.gameObject))
        {
            Debug.LogWarning("2 " + other.gameObject.name + " trigger entered");
            return;
        }

        // Sprawdzenie, czy obiekt to PickableObject i czy nie jest w interakcji
        var pickable = other.GetComponent<PickableObject>();
        if (pickable != null && pickable.IsInteracting)
        {
            Debug.LogWarning("Obiekt " + other.gameObject.name + " jest w interakcji i nie zostanie dodany do listy.");
            return;
        }

        Debug.LogWarning("3");
        pressingObjects.Add(other.gameObject);
        Debug.LogWarning("4");

        if (!IsUsed)
        {
            Debug.LogWarning("5");
            ActivatePlate();
        }

        if (TwoSideInteraction && checkCoroutine == null)
        {
            Debug.LogWarning("6");
            checkCoroutine = StartCoroutine(CheckPlateStatus());
        }
    }


    private IEnumerator CheckPlateStatus()
    {
        Debug.LogWarning("object count " + pressingObjects.Count);

        while (pressingObjects.Count > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        DeactivatePlate();
        checkCoroutine = null;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.LogWarning("1E");
        if (!IsValidObject(other.gameObject))
        {
            Debug.LogWarning("2E");
            return;
        }

        Debug.LogWarning("3E");
        pressingObjects.Remove(other.gameObject);
    }

    private bool IsValidObject(GameObject obj)
    {
        return obj.GetComponent<PlayerBase>() != null || obj.GetComponent<PickableObject>() != null;
    }

    private void ActivatePlate()
    {
        objectToInteract.IsBlocked = false;
        objectToInteract.CanInteract = true;

        InteractEvent.Raise(this, null);
        objectToInteract?.Interact(transform);
        objectToInteract.CanInteract = false;

        plateTransform.DOMoveY(initialPosition.y - pressDepth, pressDuration);

        IsUsed = true;
        PlayPressSound();
    }

    private void DeactivatePlate()
    {
        if (!CanInteract)
            return;

        plateTransform.DOMoveY(initialPosition.y, releaseDuration);

        IsUsed = false;
        PlayReleaseSound();

        if (objectToInteract != null)
        {
            objectToInteract.CanInteract = true;
            objectToInteract.Interact(transform);
            objectToInteract.IsBlocked = true;
        }
    }

    private void PlayPressSound()
    {
        pressSoundInstance = RuntimeManager.CreateInstance(pressSoundEvent);
        if (soundSource != null)
            RuntimeManager.AttachInstanceToGameObject(pressSoundInstance, soundSource);

        pressSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        pressSoundInstance.start();
        pressSoundInstance.release();
    }

    private void PlayReleaseSound()
    {
        releaseSoundInstance = RuntimeManager.CreateInstance(releaseSoundEvent);
        if (soundSource != null)
            RuntimeManager.AttachInstanceToGameObject(releaseSoundInstance, soundSource);

        releaseSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        releaseSoundInstance.start();
        releaseSoundInstance.release();
    }

    public void Interact(Transform player)
    {
    }

    public void ShowUI()
    {
    }

    public void HideUI()
    {
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PressurePlateController : MonoBehaviour, IInteractable
{
    [Header("Reference")] [SerializeField] private GameObject gameobjectToInteract;
    private IInteractable objectToInteract;
    [SerializeField] private Animator animator;

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
    public GameEvent ShowUIEvent { get; set; } // Puste, UI nie jest obsługiwane

    public GameEvent InteractEvent
    {
        get => interactEvent;
        set => interactEvent = value;
    }

    [Header("Callbacks")] public string InteractMessage { get; set; } // Puste, UI nie jest obsługiwane
    public string BlockedMessage { get; set; } // Puste, UI nie jest obsługiwane

    private HashSet<GameObject> pressingObjects = new(); // Lista obiektów na płycie
    private Coroutine checkCoroutine;

    private void Awake()
    {
        if (gameobjectToInteract == null) return;
        if (gameobjectToInteract.TryGetComponent<IInteractable>(out IInteractable iInteractable))
        {
            objectToInteract = iInteractable;
            objectToInteract.TwoSideInteraction = twoSidesInteraction;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (IsBlocked || !CanInteract || !IsValidObject(other.gameObject))
            return;

        pressingObjects.Add(other.gameObject);

        if (!IsUsed)
            ActivatePlate();

        if (TwoSideInteraction && checkCoroutine == null)
            checkCoroutine = StartCoroutine(CheckPlateStatus());
    }

    private IEnumerator CheckPlateStatus()
    {
        while (pressingObjects.Count > 0)
        {
            Debug.Log("Checking plate status");
            yield return new WaitForSeconds(1f); // Co 0.5s sprawdzamy, czy coś jest na płycie
        }

        DeactivatePlate();
        checkCoroutine = null;
    }

    private void OnCollisionExit(Collision other)
    {
        if (!IsValidObject(other.gameObject))
            return;

        pressingObjects.Remove(other.gameObject);
    }

    private bool IsValidObject(GameObject obj)
    {
        return obj.GetComponent<PlayerBase>() != null || obj.GetComponent<PickableObject>() != null;
    }

    private void ActivatePlate()
    {
        InteractEvent.Raise(this, null);

        objectToInteract?.Interact(transform);

        objectToInteract.CanInteract = false;

        if (animator != null)
            animator.SetTrigger("Press");

        IsUsed = true;
        PlayPressSound();
    }

    private void DeactivatePlate()
    {
        Debug.Log("desactivate plate 1");
        if (!CanInteract)
            return;

        Debug.Log("desactivate plate 2");

        if (animator != null)
            animator.SetTrigger("Release");

        Debug.Log("desactivate plate 3");

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
        //empty
    }

    public void ShowUI()
    {
        //empty

    } 

    public void HideUI()
    {
        //empty
    } 
}
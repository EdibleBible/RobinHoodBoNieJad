using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputInteract : MonoBehaviour
{
    public List<IInteract> objectsInReach = new();
    public IInteract closestObjectBase;
    public PlayerBase playerBase;
    public InputActionAsset globalInputActions;
    private InputAction interactionAction;
    private Vector3 playerPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IInteract interactiveBase))
        {
            objectsInReach.Add(interactiveBase);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        bool isInteractable = other.gameObject.TryGetComponent(out IInteract interactiveBase);
        if (isInteractable && objectsInReach.Contains(interactiveBase))
        {
            objectsInReach.Remove(interactiveBase);
        }
    }

    private void LateUpdate()  
    {
        if (objectsInReach.Count > 1)
        {
            playerPosition = playerBase.position;
            float newShortestObj;
            float shortestObj = Vector3.Distance(objectsInReach[0].gameObject.transform.position, playerPosition);
            closestObjectBase = objectsInReach[0];
            for (var i = 1; i < objectsInReach.Count; i++)
            {
                newShortestObj = Vector3.Distance(objectsInReach[i].gameObject.transform.position, playerPosition);
                if (newShortestObj < shortestObj)
                {
                    shortestObj = newShortestObj;
                    closestObjectBase = objectsInReach[i];
                }
            }
        } else if (objectsInReach.Count == 1) { closestObjectBase = objectsInReach[0]; }
    }

    private void Awake()
    {
        interactionAction = globalInputActions.FindAction("Interact");
        interactionAction.Enable();
        interactionAction.started += HandleInteraction;
    }

    private void OnDisable()
    {
        interactionAction.started -= HandleInteraction;
        interactionAction.Disable();
    }

    private void HandleInteraction(InputAction.CallbackContext context)
    {
        if (closestObjectBase.Interact(playerBase))
        {
            closestObjectBase = null;
        }
    }
}

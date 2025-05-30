using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private PlayerBase playerBase;
    [SerializeField] private Transform raycasterTransform;
    [SerializeField] private Transform holdPositionTransform;
    [SerializeField] private float interactionDistance;
    [SerializeField] private float sphereRadius;
    [SerializeField] private LayerMask interactableLayer;
    private IInteractable currentInteractable;
    private RaycastHit hitInfo;

    private void Update()
    {
        if (Physics.SphereCast(raycasterTransform.position, sphereRadius, raycasterTransform.forward, out hitInfo, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hitInfo.collider.GetComponentInParent<IInteractable>();

            if (interactable != null && interactable.CanInteract && currentInteractable != interactable)
            {
                currentInteractable?.HideUI();

                if (interactable is IInteractableStop interactableStop && interactableStop.IsInteracting)
                    interactableStop.ShowStopUI();
                else
                    interactable.ShowUI();

                currentInteractable = interactable;
            }
        }
        else if (currentInteractable is IInteractableStop interactableStop && !interactableStop.IsInteracting)
        {
            currentInteractable.HideUI();
            currentInteractable = null;
        }
        else if (currentInteractable != null && !(currentInteractable is IInteractableStop))
        {
            currentInteractable.HideUI();
            currentInteractable = null;
        }
    }

    public void Interact()
    {
        if (currentInteractable != null)
        {
            Debug.Log("Interacted with " + currentInteractable);
            currentInteractable.Interact(transform);
        }
    }

    public void StopInteracting()
    {
        if (currentInteractable != null && currentInteractable is IInteractableStop stop)
        {
            Debug.Log("Interacted with " + currentInteractable);
            stop.StopInteracting();
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = hitInfo.collider == null ? Color.red : Color.green;
        Vector3 start = raycasterTransform.position;
        Vector3 end = start + raycasterTransform.forward * (hitInfo.collider == null ? interactionDistance : hitInfo.distance);
    
        Gizmos.DrawWireSphere(start, sphereRadius);
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, sphereRadius);
    }

    public Transform GetHoldPosition()
    {
        return holdPositionTransform;
    }
}
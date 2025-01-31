using System;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private Transform raycasterTransform;
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactionKey;
    private IInteractable currentInteractable;
    private RaycastHit hitInfo;

    private void Update()
    {
        if (Physics.Raycast(raycasterTransform.position, raycasterTransform.forward, out hitInfo, interactionDistance,
                interactableLayer))
        {
            if (hitInfo.collider.gameObject.TryGetComponent<IInteractable>(out var interactable))
            {
                if (interactable.CanInteract)
                {
                    if (currentInteractable == null)
                        interactable.ShowUI();
                    else
                    {
                        currentInteractable.HideUI();
                        interactable.ShowUI();
                    }

                    currentInteractable = interactable;
                }
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.HideUI();
                currentInteractable = null;
            }
        }


        if (currentInteractable != null && Input.GetKeyDown(interactionKey))
        {
            currentInteractable.Interact(transform);
        }
    }

    private void OnDrawGizmos()
    {
        if (hitInfo.collider == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(raycasterTransform.position, raycasterTransform.forward * interactionDistance);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(raycasterTransform.position, raycasterTransform.forward * hitInfo.distance);
        }
    }
}
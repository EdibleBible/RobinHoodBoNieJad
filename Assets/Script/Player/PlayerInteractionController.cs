using System;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private PlayerBase playerBase;
    [SerializeField] private Transform raycasterTransform;
    [SerializeField] private float interactionDistance;
    [SerializeField] private float sphereRadius;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactionKey;
    [SerializeField] private KeyCode dropKey;
    private IInteractable currentInteractable;
    private RaycastHit hitInfo;

    private void Update()
    {
        if (Physics.SphereCast(raycasterTransform.position, sphereRadius, raycasterTransform.forward, out hitInfo, interactionDistance, interactableLayer))
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
            Debug.Log("Interacted with " + currentInteractable);
            currentInteractable.Interact(transform);
        }

        if (Input.GetKeyDown(dropKey))
        {
            Debug.Log("Drop 1");
            playerBase.DropItem();
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
}
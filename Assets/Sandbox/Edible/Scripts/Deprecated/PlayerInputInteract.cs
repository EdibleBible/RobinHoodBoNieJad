using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputInteract : MonoBehaviour
{
    /*[HideInInspector] public PlayerBase playerBase;

    //Handles indexing objects reachable by the player
    public List<IInteract> objectsInReach = new();
    public IInteract closestObjectBase;
    private void OnTriggerEnter(Collider other) //Indexing reachable objects
    {
        if (other.gameObject.TryGetComponent(out IInteract interactiveBase))
        {
            objectsInReach.Add(interactiveBase);
        }
    }

    private void OnTriggerExit(Collider other) //Un-indexing reachable objects
    {
        bool isInteractable = other.gameObject.TryGetComponent(out IInteract interactiveBase);
        if (isInteractable && objectsInReach.Contains(interactiveBase))
        {
            objectsInReach.Remove(interactiveBase);
        }
    }
    private void LateUpdate() //Updates ref for closest object every frame
    {
        if (objectsInReach.Count > 1) //Skips if only one item in reach
        {
            Vector3 playerPosition = playerBase.position;
            float newShortestObj;
            float shortestObj = Vector3.Distance(objectsInReach[0].gameObject.transform.position, playerPosition);
            closestObjectBase = objectsInReach[0]; //Indexes first item
            for (var i = 1; i < objectsInReach.Count; i++) //Skips first item
            {
                newShortestObj = Vector3.Distance(objectsInReach[i].gameObject.transform.position, playerPosition);
                if (newShortestObj < shortestObj)
                {
                    shortestObj = newShortestObj;
                    closestObjectBase = objectsInReach[i];
                }
            }
        }
        else if (objectsInReach.Count == 1) { closestObjectBase = objectsInReach[0]; }
    }

    //Handles Interaction keybind event
    public InputActionAsset globalInputActions; 
    private InputAction interactionAction; 

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
            closestObjectBase = null; //Clears reference to avoid multiple interaction in one frame
        }
    }*/
}

using System;
using UnityEngine;

public class PlessurePlateController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject gameobjectToInteract;
    private IInteractable InteractElement;
    
    [SerializeField] private bool canPress = true;
    [SerializeField] private bool twoSidePress;
    private bool isPress = false;

    private void Awake()
    {
        InteractElement = gameobjectToInteract.GetComponent<IInteractable>();
        
        InteractElement.CanInteract = true;
        InteractElement.IsBlocked = false;
    }

    public void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision");
        if (!canPress)
        {
            Debug.Log("Collision cant press");
            return;
        }
        if (other.gameObject.GetComponent<PickableObject>())
        {
            Debug.Log("Collision eneter correct");
            PressPlate();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log("Collision exit");

        if (!canPress)
        {
            Debug.Log("Collision cant exit");
            return;
        }
        if (other.gameObject.GetComponent<PickableObject>())
        {
            Debug.Log("Collision exit correct");
            ReleasePlate();
        }
    }

    public void PressPlate()
    {
        if (!isPress)
        {
            Debug.Log("Pressing Plate");
            isPress = true;
            InteractElement.Interact(null);
        }
        else
        {
            Debug.Log("Cant Pressing Plate");
        }
    }

    public void ReleasePlate()
    {
        if (!twoSidePress)
        {
            Debug.Log("Releasing Plate");
            InteractElement.TwoSideInteraction = false;
            return;
        }


        if (isPress)
        {
            Debug.Log("Releasing Plate");
            isPress = false;
            InteractElement.Interact(null);
            InteractElement.TwoSideInteraction = true;
            InteractElement.Interact(null);
        }
        else
        {
            Debug.Log("cant Releasing Plate");
        }
    }
}
using System;
using UnityEngine;

public class NextInteractTutorialTrigger : MonoBehaviour
{
    private IInteractable interactable;

    private void Awake()
    {
        // Znalezienie komponentu implementującego IInteractable
        interactable = GetComponent<IInteractable>();

        if (interactable != null)
        {
            Debug.Log("Znaleziono komponent implementujący IInteractable.");
        }
        else
        {
            Debug.LogWarning("Nie znaleziono komponentu implementującego IInteractable na obiekcie: " + gameObject.name);
        }
    }

    public void CheckInteractEvent(Component sender, object data)
    {
        if (sender is IInteractable interactable && interactable == this.interactable)
        {
            TriggerSelectedTutorial();
        }
    }
    

    public void TriggerSelectedTutorial()
    {
        if (TutorialEventManager.Instance != null)
        {
            TutorialEventManager.Instance.NextTutorialElement();
        }
        Destroy(this);
    }
}

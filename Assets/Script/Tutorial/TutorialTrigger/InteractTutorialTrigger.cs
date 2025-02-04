using System;
using UnityEngine;

public class InteractTutorialTrigger : MonoBehaviour , ITutorialEvent
{
    [SerializeField] private string eventName; // Nazwa eventu przypisanego do triggera
    public string EventName => eventName;

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
        Debug.Log($"Wejście w trigger: {EventName}");
        TutorialEventManager.Instance.StartTutorial(EventName);  // Triggerowanie eventu w menedżerze
    }
}

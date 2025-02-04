using UnityEngine;

public class TriggerTutorial : MonoBehaviour, ITutorialEvent
{
    [SerializeField] private string eventName; // Nazwa eventu przypisanego do triggera
    public string EventName => eventName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Warunek wejścia gracza
        {
            TriggerSelectedTutorial();
        }
        
        Destroy(this);
    }

    public void TriggerSelectedTutorial()
    {
        Debug.Log($"Wejście w trigger: {eventName}");
        TutorialEventManager.Instance.StartTutorial(eventName);  // Triggerowanie eventu w menedżerze
    }
}
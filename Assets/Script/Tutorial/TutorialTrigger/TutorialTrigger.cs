using UnityEngine;

public class TutorialTrigger : MonoBehaviour, ITutorialEvent
{
    [SerializeField] private string eventName = "Part1";
    public string EventName => eventName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Sprawdza, czy gracz wchodzi do triggera
        {
            TriggerTutorial();
        }
    }

    public void TriggerTutorial()
    {
        Debug.Log($"Wywołano tutorial: {EventName}");
        TutorialEventManager.Instance.StartTutorial(EventName);
    }
}
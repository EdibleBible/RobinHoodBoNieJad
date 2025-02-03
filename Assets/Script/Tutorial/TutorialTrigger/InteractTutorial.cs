using UnityEngine;

public class InteractTutorial : MonoBehaviour, ITutorialEvent
{
    /*[SerializeField] private string eventName; // Nazwa eventu np. "TalkToOldMan"

    public void TriggerEvent()
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning($"[{gameObject.name}] Brak nazwy eventu w InteractTutorial!");
            return;
        }

        Debug.Log($"Rozmowa z NPC: {eventName}");
        TutorialEventManager.Instance.StartTutorial(eventName);

        // Opcjonalnie wyłącz komponent po aktywacji
        enabled = false;
    }

    public void TriggerEvent()
    {
        throw new System.NotImplementedException();
    }*/
    public void TriggerTutorial()
    {
        throw new System.NotImplementedException();
    }
}
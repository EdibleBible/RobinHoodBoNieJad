using UnityEngine;

public class ItemPickupTutorial : MonoBehaviour, ITutorialEvent
{
    /*[SerializeField] private string eventName; // Nazwa eventu przypisanego do przedmiotu

    public void TriggerEvent()
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning($"[{gameObject.name}] Brak nazwy eventu w ItemPickupTutorial!");
            return;
        }

        Debug.Log($"Podniesiono {gameObject.name}, odpalam event {eventName}");
        TutorialEventManager.Instance.StartTutorial(eventName);

        // Opcjonalnie wyłączenie komponentu lub całego obiektu po aktywacji
        gameObject.SetActive(false);
    }*/
    public void TriggerTutorial()
    {
        throw new System.NotImplementedException();
    }
}
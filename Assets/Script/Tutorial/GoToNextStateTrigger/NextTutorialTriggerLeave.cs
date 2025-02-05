using UnityEngine;

public class NextTutorialTriggerLeave : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Sprawdzamy, czy gracz wszedł
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
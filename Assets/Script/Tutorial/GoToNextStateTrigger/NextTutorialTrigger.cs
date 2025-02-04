using UnityEngine;

public class NextTutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Sprawdzamy, czy gracz wszedł
        {
            if (TutorialEventManager.Instance != null)
            {
                TutorialEventManager.Instance.NextTutorialElement();
            }
            Destroy(gameObject); // Usuwamy trigger po aktywacji (opcjonalne)
        }
    }
}
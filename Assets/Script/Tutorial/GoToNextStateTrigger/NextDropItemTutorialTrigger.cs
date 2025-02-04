using UnityEngine;

public class NextDropItemTutorialTrigger : MonoBehaviour
{
    public void CheckInteractEvent(Component sender, object data)
    {
        TriggerSelectedTutorial();
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
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialClaster
{
    public string EventName;  // Nazwa eventu dla lepszego sterowania.
    public List<TutorialDataHolder> TutorialElements = new List<TutorialDataHolder>();
    private int currTutorialIndex = -1;
    public bool isAutoProgress = true;  // Czy przejście do kolejnego elementu ma się odbywać automatycznie po kliknięciu
    public event Action OnTutorialCompleted; // Wywoływane, gdy zakończy się cały klaster.

    public void HideClaster()
    {
        foreach (var element in TutorialElements)
        {
            element.HideTutorial();
        }
    }

    public void ResetClaster()
    {
        currTutorialIndex = -1;
        HideClaster();
    }

    public void GoToNextTutorialInClaster()
    {
        if (TutorialElements.Count == 0)
        {
            Debug.LogWarning("Brak elementów w klastrze tutoriali.");
            return;
        }

        // Ukryj obecny element (jeśli nie jest to pierwsze wywołanie)
        if (currTutorialIndex >= 0 && currTutorialIndex < TutorialElements.Count)
        {
            TutorialElements[currTutorialIndex].HideTutorial();
        }

        currTutorialIndex++;

        if (currTutorialIndex >= TutorialElements.Count)
        {
            Debug.Log($"Zakończono klaster tutoriali: {EventName}");
            OnTutorialCompleted?.Invoke(); // Powiadomienie o zakończeniu tutorialu
            return;
        }

        TutorialElements[currTutorialIndex].ChangeTutorialText();
    }

    public void TriggerNextTutorialManually()
    {
        if (!isAutoProgress)
        {
            GoToNextTutorialInClaster();
        }
    }
}

[Serializable]
public class TutorialDataHolder
{
    public TutorialElement TutorialElement;
    [TextArea] public string TutorialText;

    public void ChangeTutorialText()
    {
        if (TutorialElement != null)
        {
            TutorialElement.ShowTutorial(TutorialText);
        }
        else
        {
            Debug.LogWarning("Brak przypisanego elementu tutorialu.");
        }
    }

    public void HideTutorial()
    {
        if (TutorialElement != null)
        {
            TutorialElement.HideTutorial();
        }
    }
}

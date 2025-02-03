using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialClaster
{
    public string EventName;
    public List<TutorialDataHolder> TutorialElements = new List<TutorialDataHolder>();
    private int currTutorialIndex = -1;

    public event Action OnTutorialCompleted;

    public void HideClaster()
    {
        if (TutorialElements.Count == 0)
        {
            Debug.LogWarning($"[{EventName}] Brak elementów do ukrycia.");
            return;
        }

        foreach (var element in TutorialElements)
        {
            element.HideTutorial();
        }
    }

    public void ResetClaster()
    {
        if (currTutorialIndex >= 0 && currTutorialIndex < TutorialElements.Count)
        {
            HideClaster();
        }
        currTutorialIndex = -1;
    }

    public void GoToNextTutorialInClaster()
    {
        if (TutorialElements.Count == 0)
        {
            Debug.LogWarning($"[{EventName}] Brak elementów w klastrze tutoriali.");
            return;
        }

        if (currTutorialIndex >= 0 && currTutorialIndex < TutorialElements.Count)
        {
            TutorialElements[currTutorialIndex].HideTutorial();
        }

        currTutorialIndex++;

        if (currTutorialIndex >= TutorialElements.Count)
        {
            Debug.Log($"[{EventName}] Zakończono klaster tutoriali.");
            OnTutorialCompleted?.Invoke();
            ResetClaster();  // Resetujemy klaster po zakończeniu
            return;
        }

        TutorialElements[currTutorialIndex].ChangeTutorialText();
    }
}

[Serializable]
public class TutorialDataHolder
{
    public TutorialElement TutorialElement;
    [TextArea] public string TutorialText;

    public void ChangeTutorialText()
    {
        if (TutorialElement == null)
        {
            Debug.LogWarning($"Brak przypisanego elementu tutorialu! Nie można wyświetlić tekstu: {TutorialText}");
            return;
        }
        
        TutorialElement.ShowTutorial(TutorialText);
    }

    public void HideTutorial()
    {
        if (TutorialElement != null)
        {
            TutorialElement.HideTutorial();
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialClaster
{
    public string EventName;  
    public List<TutorialDataHolder> TutorialElements = new List<TutorialDataHolder>();
    private int currTutorialIndex = -1;
    public bool isAutoProgress = true;  
    public event Action OnTutorialCompleted;

    private bool isWaitingForNext = false;
    private float timer = 0f;
    private float waitTime = 0f;

    public void HideClaster()
    {
        foreach (var element in TutorialElements)
        {
            element.HideTutorial();
        }
        isWaitingForNext = false;
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

        if (currTutorialIndex >= 0 && currTutorialIndex < TutorialElements.Count)
        {
            TutorialElements[currTutorialIndex].HideTutorial();
        }

        currTutorialIndex++;

        if (currTutorialIndex >= TutorialElements.Count)
        {
            Debug.Log($"Zakończono klaster tutoriali: {EventName}");
            OnTutorialCompleted?.Invoke();
            return;
        }

        TutorialElements[currTutorialIndex].ChangeTutorialText();

        // Sprawdzenie, czy dany tutorial jest tymczasowy
        if (TutorialElements[currTutorialIndex].TempolaryTutorial)
        {
            waitTime = TutorialElements[currTutorialIndex].Time;
            timer = 0f;
            isWaitingForNext = true;
        }
    }

    public void Update()
    {
        if (isWaitingForNext)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                isWaitingForNext = false;
                GoToNextTutorialInClaster();
            }
        }
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
    public bool TempolaryTutorial;
    public float Time;

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


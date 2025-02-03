using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventManager : MonoBehaviour
{
    public static TutorialEventManager Instance { get; private set; }

    [SerializeField] private List<TutorialClaster> tutorialClusters = new List<TutorialClaster>();
    private TutorialClaster currentClaster;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            NextTutorialElement();
        }
    }

    public void StartTutorial(string eventName)
    {
        TutorialClaster claster = tutorialClusters.Find(t => t.EventName == eventName);

        if (claster != null)
        {
            Debug.Log($"[TutorialEventManager] Rozpoczynam klaster: {eventName}");
            currentClaster = claster;
            currentClaster.GoToNextTutorialInClaster();  // Start pierwszego elementu
        }
        else
        {
            Debug.LogWarning($"[TutorialEventManager] Nie znaleziono tutorialu: {eventName}");
        }
    }

    public void NextTutorialElement()
    {
        if (currentClaster != null)
        {
            Debug.Log("[TutorialEventManager] Przechodzę do kolejnego elementu w klastrze.");
            currentClaster.GoToNextTutorialInClaster();
        }
    }
}


public interface ITutorialEvent
{
    void TriggerTutorial();
}
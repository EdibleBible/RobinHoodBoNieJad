using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

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
        if(currentClaster == null)
            return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0)  && !currentClaster.isAutoProgress)
        {
            NextTutorialElement();
        }
        
        currentClaster.Update();
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

    public void TriggerTutorialEvent(string eventName)
    {
        // Znajdź event po nazwie i wyzwól go
        var tutorialEvent = FindObjectsOfType<MonoBehaviour>()
                .OfType<ITutorialEvent>() // <- LINQ (potrzebny using System.Linq)
                .FirstOrDefault(t => t.EventName == eventName);

        if (tutorialEvent != null)
        {
            tutorialEvent.TriggerSelectedTutorial();
        }
        else
        {
            Debug.LogWarning($"Nie znaleziono eventu: {eventName}");
        }
    }
    
    
}
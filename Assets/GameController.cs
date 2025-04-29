using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [HideInInspector] public static GameController Instance { get; private set; }
    
    public SOAllQuest AllPlayerQuest;
    public bool DebugMode;


    private void Awake()
    {
        if (DebugMode)
        {
            StartNewGame();
            AllPlayerQuest.CurrentSelectedQuest = AllPlayerQuest.randomizedQuests[0];
        }
        
        // Singleton Init
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // zachowuje między scenami
        

        
    }

    public void StartNewGame()
    {
        AllPlayerQuest.RandomizeAllQuests();
    }

    public void RandomizeQuest()
    {
        AllPlayerQuest.CurrentSelectedQuest = AllPlayerQuest.randomizedQuests[0];
        Debug.Log("Randomize quest: " + AllPlayerQuest.CurrentSelectedQuest.Description);
    }

    public void LoadGame(object data)
    {
        //TODO: metoda tylko do wglądu trzeba ja potem zrobić
        AllPlayerQuest.LoadAllQuest();
    }
}

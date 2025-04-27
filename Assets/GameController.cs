using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [HideInInspector] public static GameController Instance { get; private set; }
    public SOAllQuest AllPlayerQuest;


    private void Awake()
    {
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

    public void LoadGame(object data)
    {
        //TODO: metoda tylko do wglądu trzeba ja potem zrobić
        AllPlayerQuest.LoadAllQuest();
    }
}

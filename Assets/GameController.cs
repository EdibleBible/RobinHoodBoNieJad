using Script.ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    [HideInInspector] public static GameController Instance { get; private set; }

    public SOAllQuest AllPlayerQuest;
    public SOInventory PlayerInventory;
    public SOPlayerStatsController PlayerStatsController;
    
    public bool DebugMode;
    public bool DontCleanInventory;
    public bool ResetAllStatsModifiers;
    public bool RemoveAllBaseStatModifiers;

    public ScriptableRendererFeature fullScreenPassFeature; // Przypisz w Inspectorze

    private void Awake()
    {
        if (DebugMode)
        {
            StartNewGame();
            AllPlayerQuest.CurrentSelectedQuest = AllPlayerQuest.randomizedQuests[0];
        }

        if (ResetAllStatsModifiers)
        {
            PlayerStatsController.ResetAllModifiers();
        }

        if (RemoveAllBaseStatModifiers)
        {
            PlayerStatsController.RemoveAllBaseModier();
        }

        // Singleton Init
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ToggleFullScreenPass(false);

        Instance = this;
        DontDestroyOnLoad(gameObject); // zachowuje między scenami
    }

    public void CleanUpInventory()
    {
        if (!DontCleanInventory)
            PlayerInventory.ClearInventory();
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

    public void ToggleFullScreenPass()
    {
        if (fullScreenPassFeature != null)
        {
            fullScreenPassFeature.SetActive(!fullScreenPassFeature.isActive);
            Debug.Log("FullScreenPassRendererFeature: " + fullScreenPassFeature.isActive);
        }
    }

    public void ToggleFullScreenPass(bool state)
    {
        if (fullScreenPassFeature != null)
        {
            fullScreenPassFeature.SetActive(state);
            Debug.Log("FullScreenPassRendererFeature: " + fullScreenPassFeature.isActive);
        }
    }

    private void OnApplicationQuit()
    {
        ToggleFullScreenPass(false);
    }

    public void ToogleCursorOn()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ToogleCursorOff()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

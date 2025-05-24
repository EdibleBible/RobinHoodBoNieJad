using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MainMenuStartController : MonoBehaviour
{
    public Button loadButton;
    public TextMeshProUGUI loadingText;

    public Color disableColor;
    public Color enableColor;
    void Awake()
    {
        string path = Path.Combine(Application.persistentDataPath, "savegame.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            loadButton.interactable = true;
            loadingText.color = enableColor;
            Debug.Log("Zapis znaleziony:\n" + json);
        }
        else
        {
            loadButton.interactable = false;
            loadingText.color = disableColor;
            Debug.LogWarning("Plik savegame.json nie istnieje.");
        }
    }

    public void StartNewGame()
    {
        GameController.Instance.CleanUpInventory();
        GameController.Instance.PlayerStatsController.ResetAllModifiers();
        GameController.Instance.PlayerStatsController.RemoveAllBaseModier();
        
        GameController.Instance.StartNewGame();
    }

    public void LoadGame()
    {
        GameController.Instance.CleanUpInventory();
        GameController.Instance.PlayerStatsController.ResetAllModifiers();
        GameController.Instance.PlayerStatsController.RemoveAllBaseModier();
        
        GameController.Instance.LoadGame();
    }
}
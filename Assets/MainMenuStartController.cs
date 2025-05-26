using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MainMenuStartController : MonoBehaviour
{
    [FormerlySerializedAs("loadButton")] public Button LoadButton;
    [FormerlySerializedAs("loadingText")] public TextMeshProUGUI LoadingText;
    public GameObject StartTutorialPanel;

    public Color disableColor;
    public Color enableColor;
    void Awake()
    {
        string path = Path.Combine(Application.persistentDataPath, "savegame.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LoadButton.interactable = true;
            LoadingText.color = enableColor;
            Debug.Log("Zapis znaleziony:\n" + json);
        }
        else
        {
            LoadButton.interactable = false;
            LoadingText.color = disableColor;
            Debug.LogWarning("Plik savegame.json nie istnieje.");
        }
    }

    public void OpenTutorialPanel()
    {
        StartTutorialPanel.SetActive(true);
    }

    public void CloseTutorialPanel()
    {
        StartTutorialPanel.SetActive(false);
    }

    public void StartTutorial()
    {
        GameController.Instance.CleanUpInventory();
        GameController.Instance.PlayerStatsController.ResetAllModifiers();
        GameController.Instance.PlayerStatsController.RemoveAllBaseModier();
        
        GameController.Instance.StartTutorial();
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
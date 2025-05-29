using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class MainMenuStartController : MonoBehaviour
{
    [FormerlySerializedAs("loadButton")] public Button LoadButton;
    public GameObject StartTutorialPanel;
    public GameObject LoadingScreen;
    public string sceneToUnload;

    void Awake()
    {
        string path = Path.Combine(Application.persistentDataPath, "savegame.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LoadButton.interactable = true;
            Debug.Log("Zapis znaleziony:\n" + json);
        }
        else
        {
            LoadButton.interactable = false;
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

    public void LoadingScene(int sceneIndex)
    {
        StartCoroutine(LoadGameWithFade(sceneIndex, sceneToUnload));
    }

    
    [SerializeField] private List<GameObject> objToOff = new List<GameObject>();
    public Slider progressBar;
    public TextMeshProUGUI continueText;
    public float minLoadTime = 1f;
    private bool isReadyToContinue = false;

    
    private IEnumerator LoadGameWithFade(int gameSceneIndex, string lobbySceneName)
    {
        LoadingScreen.SetActive(true);

        yield return StartCoroutine(Fade(1));

        foreach (var obj in objToOff)
        {
            obj.SetActive(false);
        }

        progressBar.gameObject.SetActive(true);
        continueText.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AsyncOperation gameLoadOperation = SceneManager.LoadSceneAsync(gameSceneIndex, LoadSceneMode.Additive);
        gameLoadOperation.allowSceneActivation = false;

        float elapsedTime = 0f;
        while (!gameLoadOperation.isDone)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(gameLoadOperation.progress / 0.9f);
            progressBar.value = progress;

            if (progress >= 1f && elapsedTime >= minLoadTime && !isReadyToContinue)
            {
                // Scena załadowana, czekamy na input
                progressBar.gameObject.SetActive(false);
                continueText.text = "Press any key to continue...";
                continueText.gameObject.SetActive(true);
                isReadyToContinue = true;
            }

            if (isReadyToContinue && Input.anyKeyDown)
            {
                gameLoadOperation.allowSceneActivation = true;
                yield return StartCoroutine(Fade(0));
                SceneManager.UnloadSceneAsync(lobbySceneName);
                yield break; // kończymy coroutine po aktywacji sceny
            }

            yield return null;
        }
    }

    public float fadeDuration = 0.5f;
    public List<CanvasGroup> fadeCanvasGroup;

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup[0].alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            foreach (var group in fadeCanvasGroup)
            {
                group.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);

            }
            time += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup[0].alpha = targetAlpha;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;

public class MenuButtonLobby : MonoBehaviour
{
    public GameObject loadingScreen;
    public Image backgroundImage;
    public Slider progressBar;
    public TextMeshProUGUI continueText;
    public float minLoadTime = 1f;
    public List<CanvasGroup> fadeCanvasGroup;
    public float fadeDuration = 0.5f;
    private bool isReadyToContinue = false;
    [SerializeField] private List<GameObject> objToOff = new List<GameObject>();
    public SOInventory inventory;

    private void Awake()
    {
        inventory.CurrInvenoryScore += inventory.ScoreBackup;
        inventory.ScoreBackup = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Open(GameObject lobbyObject)
    {
        // Zatrzymanie d�wi�ku przy zmianie canvasu
        StopAllHoverSounds();
        lobbyObject.SetActive(true);
    }

    private void StopAllHoverSounds()
    {
        // ten skrypt nie działa xD nie wiem czyu to merge czy co ale ta część nie działa (nie ma instancji klasy/Metody HoverMouse), w skrócie nie istnieje ten obiekt ~~TED
        /*
        HoverMouse.OnSceneChange();
    */
    }


    public void Close(GameObject lobbyObject)
    {
        lobbyObject.SetActive(false);
    }

    public void StartGame()
    {
        inventory.ScoreBackup = inventory.CurrInvenoryScore;
        StartCoroutine(LoadGameWithFade(2, "Lobby"));
    }

    private IEnumerator LoadGameWithFade(int gameSceneIndex, string lobbySceneName)
    {
        loadingScreen.SetActive(true);
        
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

            if (progress >= 1f && elapsedTime >= minLoadTime)
            {
                progressBar.gameObject.SetActive(false);
                isReadyToContinue = true;
                gameLoadOperation.allowSceneActivation = true;
            }

            
            if (isReadyToContinue)
            {
                yield return StartCoroutine(Fade(0));
                SceneManager.UnloadSceneAsync(lobbySceneName);
                yield return null;
            }

            yield return null;
        }
    }

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
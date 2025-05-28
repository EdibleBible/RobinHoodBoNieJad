using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUiController : MonoBehaviour
{
    public GameObject loadingScreen;
    public float minLoadTime = 1f;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;
    private bool isReadyToContinue = false;
    [SerializeField] private List<GameObject> objToOff = new List<GameObject>();
    public string sceneToLoad;
    public string sceneToUnload;
    private bool isUSed;

    public void Open(GameObject lobbyObject)
    {
        lobbyObject.SetActive(true);
    }

    public void Close(GameObject lobbyObject)
    {
        lobbyObject.SetActive(false);
    }

    public void GameOver(Component sender, object data)
    {
        Debug.Log("fhuJ");
        GameController gameController = GameController.Instance;
        
        if (isUSed == false)
        {
            gameController.ToggleFullScreenPass(false);
            isUSed = true;
            StartCoroutine(LoadGameWithFade(sceneToLoad, sceneToUnload));
        }
    }

    private IEnumerator LoadGameWithFade(string gameSceneName, string lobbySceneName)
    {
        loadingScreen.SetActive(true);
        yield return StartCoroutine(Fade(1, fadeDuration));

        foreach (var obj in objToOff)
        {
            obj.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AsyncOperation gameLoadOperation = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        gameLoadOperation.allowSceneActivation = false;

        float elapsedTime = 0f;
        while (!gameLoadOperation.isDone)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(gameLoadOperation.progress / 0.9f);

            if (progress >= 1f && elapsedTime >= minLoadTime)
            {
                isReadyToContinue = true;
            }

            if (isReadyToContinue && Input.GetKeyDown(KeyCode.Space))
            {
                gameLoadOperation.allowSceneActivation = true;

                // Ustawienie nowej sceny jako aktywnej przed usunięciem starej
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));

                // Poczekaj na aktywację nowej sceny
                yield return new WaitForSeconds(0.1f);

                // Teraz możemy bezpiecznie wyładować starą scenę
                SceneManager.UnloadSceneAsync(lobbySceneName);
            }

            yield return null;
        }
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}
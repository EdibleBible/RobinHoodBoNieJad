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
        StartCoroutine(LoadGameWithFade(sceneToLoad,sceneToUnload));
    }

    private IEnumerator LoadGameWithFade(string gameSceneIndex, string lobbySceneName)
    {
        loadingScreen.SetActive(true);

        yield return StartCoroutine(Fade(1));

        foreach (var obj in objToOff)
        {
            obj.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AsyncOperation gameLoadOperation = SceneManager.LoadSceneAsync(gameSceneIndex, LoadSceneMode.Additive);
        gameLoadOperation.allowSceneActivation = false;

        float elapsedTime = 0f;
        while (!gameLoadOperation.isDone)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(gameLoadOperation.progress / 0.9f);
            
            if (progress >= 1f && elapsedTime >= minLoadTime)
            {
                isReadyToContinue = true;
                gameLoadOperation.allowSceneActivation = true;
            }


            if (isReadyToContinue && Input.GetKeyDown(KeyCode.Space))
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
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}
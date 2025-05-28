using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class MenuButtonLobby : MonoBehaviour
{
    public GameObject mainCanvas;
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
    public SOStats stats;
    public GameObject dialogTaxes;
    public GameObject dialogGameOver;
    public MenuLobbyTaxes taxScript;
    public string LobbySceneName;
    public GameEvent StartGameEvent;
    
    public int SceneIndexToLoad;

    public GameObject witchTalkPanel;
    public GameObject smithTalkPanel;
    public GameObject libraryTalkPanel;

    public GameObject taxPanel;
    

    private void Awake()
    {
        foreach (ItemData itemData in inventory.ItemsInInventory)
        {
            inventory.InventoryLobby.Add(itemData);
        }
        inventory.CurrInvenoryScore += inventory.ScoreBackup;
        inventory.ScoreBackup = 0;
        Debug.Log("TUTAJ");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        
        taxPanel.SetActive(false);
    }

    public void Open(GameObject lobbyObject)
    {
        // Zatrzymanie d�wi�ku przy zmianie canvasu
        StopAllHoverSounds();
        lobbyObject.SetActive(true);
    }
    
    public void CheckShopTalkPanel(string type)
    {
        if (type == "witch")
        {
            if (stats.VisitWitch)
            {
                witchTalkPanel.SetActive(false);
            }
            else
            {
                witchTalkPanel.SetActive(true);
                stats.VisitWitch = true;
            }
        }
        else if(type == "smith")
        {
            if (stats.VisitSmith)
            {
                smithTalkPanel.SetActive(false);
            }
            else
            {
                smithTalkPanel.SetActive(true);
                stats.VisitSmith = true;
            }
        }
        else if(type =="library")
        {
            if (stats.VisitLibrary)
            {
                libraryTalkPanel.SetActive(false);
            }
            else
            {
                libraryTalkPanel.SetActive(true);
                stats.VisitLibrary = true;
            }
        }
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

    public void StartButton()
    {
        if (stats.taxPaid)
        {
            stats.taxPaid = false;
            StartGame();
        } else
        {
            dialogTaxes.SetActive(true);
        }
    }

    public void StartButtonConfirm()
    {
        float taxPunishment = 1.2f;
        if (stats.scoreTotal < taxScript.taxAmount * taxPunishment)
        {
            dialogGameOver.SetActive(true);
        } else
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        stats.lobbyVisit += 1;
        inventory.ItemsInInventory.Clear();
        inventory.ItemsInInventory.AddRange(inventory.InventoryLobby);
        inventory.ScoreBackup = inventory.CurrInvenoryScore;
        StartCoroutine(LoadGameWithFade(SceneIndexToLoad, LobbySceneName));
    }

    private IEnumerator LoadGameWithFade(int gameSceneIndex, string lobbySceneName)
    {
        loadingScreen.SetActive(true);
        mainCanvas.SetActive(false);

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
                StartGameEvent.Raise(this,this);
                SceneManager.UnloadSceneAsync(lobbySceneName);
                yield break; // kończymy coroutine po aktywacji sceny
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
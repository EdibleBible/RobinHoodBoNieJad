using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class LevelExit : MonoBehaviour, IInteractable
{
    [Header("Events")]
    [SerializeField]
    private GameEvent showUIEvent;
    [SerializeField]
    private GameEvent interactEvent;

    public GameEvent ShowUIEvent
    {
        get => showUIEvent;
        set => showUIEvent = value;
    }
    public GameEvent InteractEvent
    {
        get => interactEvent;
        set => interactEvent = value;
    }

    public bool TwoSideInteraction { get; set; } = false;
    [Header("Callbacks")]
    public string InteractMessage { get => interactMessage; set => interactMessage = value; }
    public string BlockedMessage { get => blockedMessage; set => blockedMessage = value; }

    public string interactMessage;
    public string blockedMessage;
    public bool IsUsed { get; set; } = false;


    public bool CanInteract { get; set; } = true;
    public bool IsBlocked { get; set; } = false;

    [SerializeField]
    private SOPlayerQuest playerQuest;
    private PlayerBase player;
    private bool isPlayer;
    private IInteractable interactableImplementation;

    public void Interact(Transform player)
    {
        Debug.Log("Interact");
        if (!playerQuest.IsQuestComplete())
        {
            Debug.Log("Need to end quest");
            showUIEvent.Raise(this, (false, BlockedMessage, true));
            return;
        }

        if (player != null)
        {
            Debug.Log("exit level");
            SoundManager.Instance?.StopAllActiveEvents();

            var emitters = FindObjectsByType<FMODUnity.StudioEventEmitter>(FindObjectsSortMode.None);
            foreach (var emitter in emitters)
            {
                if (emitter.IsPlaying())
                {
                    emitter.Stop();
                }
            }


            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(1);
        }
    }

    public void ShowUI()
    {
        var textToShow = InputManager.Instance.CompereTextWithInput("Interaction", interactMessage);
        ShowUIEvent.Raise(this, (true, textToShow, false));
        
    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this, (false, "", false));
    }
}
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(1);
        }
    }

    public void ShowUI()
    {
        ShowUIEvent.Raise(this, (true, InteractMessage, false));
    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this, (false, "", false));
    }
}
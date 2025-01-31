using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectLevelExit : MonoBehaviour, IUseObject , IInteractable
{
    private PlayerBase player;
    
    [Header("Events")] [SerializeField] private GameEvent showUIEvent;
    [SerializeField] private GameEvent interactEvent;
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

    public bool CanInteract { get; set; }

    [Header("Callbacks")]
    public string InteractMessage
    {
        get => interactMessage;
        set => interactMessage = value;
    }

    [SerializeField] private string interactMessage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CanInteract = true;
            ShowUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CanInteract = false;
            HideUI();
        }
    }

    public void UseObject()
    {
        if (CanInteract &&  player != null)
        {
            player.hotbar.SaveToInventory(player.inventory);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(1);
        }
    }
    
    public void Interact(Transform player)
    {
        if (CanInteract)
        {
            UseObject();
        }
    }

    public void ShowUI()
    {
        ShowUIEvent.Raise(this, (true, InteractMessage));
    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this, (false, ""));
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectLevelExit : MonoBehaviour/*, IUseObject*/
{
    /*private PlayerBase player;
    private bool isPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<PlayerBase>();
            isPlayer = true;
            ShowUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayer = false;
            HideUI();
        }
    }

    public void UseObject()
    {
        if (isPlayer &&  player != null)
        {
            player.hotbar.SaveToInventory(player.inventory);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(1);
        }
    }
    
    
    [Header("Events")] [SerializeField] private GameEvent showUIEvent;
    [SerializeField] private GameEvent interactEvent;

    [Header("Callbacks")]
    public string InteractMessage
    {
        get => interactMessage;
        set => interactMessage = value;
    }

    [SerializeField] private string interactMessage;

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

    public void ShowUI()
    {
        ShowUIEvent.Raise(this, (true, InteractMessage));
    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this, (false, ""));
    }*/
}

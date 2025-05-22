using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectLevelExit : MonoBehaviour, IUseObject
{
    private PlayerBase player;
    private bool isPlayer;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Dupa");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Dupa 1");
            player = other.gameObject.GetComponent<PlayerBase>();
            isPlayer = true;
            ShowUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Dupa 3");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Dupa 4");
            isPlayer = false;
            HideUI();
        }
    }

    public void UseObject()
    {
        if (isPlayer &&  player != null)
        {
            Debug.Log("TUTAJ");
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
        var textToShow = InputManager.Instance.CompereTextWithInput("Interaction", interactMessage);
        ShowUIEvent.Raise(this, (true, textToShow, false));
        
    }

    public void HideUI()
    {
        ShowUIEvent.Raise(this, (false, "",false));
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputEscMenu : MonoBehaviour
{
    public InputActionAsset globalInputActions;
    private InputAction escAction;
    public GameObject escapeMenu;
    public bool isScene3D;
    public Canvas canvas;
    public MenuTextButtonHover[] buttons;
    private bool forceMouse = false;

    private void AssignButtonSelectors()
    {
        foreach (var button in buttons)
        {
            if (button != null)
            {
                button.canvas = canvas;
            }
        }
    }

    private void OnEnable()
    {
        canvas = this.transform.root.GetComponent<Canvas>();
        escAction = globalInputActions.FindAction("Esc");
        escAction.Enable();
        escAction.performed += ToggleEsc;
        AssignButtonSelectors();
        forceMouse = true;
        GameController.Instance.ToogleCursorOn();
    }

    private void OnDisable()
    {
        forceMouse = false;
        escAction.performed -= ToggleEsc;
        escAction.Disable();
        GameController.Instance.ToogleCursorOff();

    }

    public void ToggleEsc(InputAction.CallbackContext context)
    {
        if (escapeMenu.activeInHierarchy)
        {
            escapeMenu.SetActive(false);
            if(isScene3D)
            {
                GameController.Instance.ToogleCursorOff();
            }
        }
        else
        {
            escapeMenu.SetActive(true);
            if(isScene3D)
            {
                GameController.Instance.ToogleCursorOn();
            }
        }
    }

    public void SceneLobby()
    {
        SceneManager.LoadScene(1);
    }

    public void SceneMenu()
    {
        GameController.Instance.SaveGameState();
        SceneManager.LoadScene(0);
    }
}
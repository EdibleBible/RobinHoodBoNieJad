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
    public GameObject OptionPanel;

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
    }

    private void OnDisable()
    {
        forceMouse = false;
        escAction.performed -= ToggleEsc;
        escAction.Disable();
    }

    public void ToggleEsc(InputAction.CallbackContext context)
    {
        if (escapeMenu.activeInHierarchy)
        {
            escapeMenu.SetActive(false);
            if (OptionPanel != null)
                OptionPanel.SetActive(false);
            if (isScene3D)
            {
                GameController.Instance.ToogleCursorOff(true);
            }
        }
        else
        {
            escapeMenu.SetActive(true);
            if (isScene3D)
            {
                GameController.Instance.ToogleCursorOn(true);
            }
        }
    }

    public void ResumeButton()
    {
        GameController.Instance.ToogleCursorOff(true);
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
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

    private void LateUpdate()
    {
        if (forceMouse)
        {
            LockMouse(false);
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
            if (isScene3D)
            {
                LockMouse(true);
            }
        }
        else
        {
            escapeMenu.SetActive(true);
            if (isScene3D)
            {
                LockMouse(false);
            }
        }
    }

    /*public void ToggleEsc()
    {
        if (escapeMenu.activeInHierarchy)
        {
            escapeMenu.SetActive(false);
        }
        else
        {
            escapeMenu.SetActive(true);
        }
    }*/

    public void SceneLobby()
    {
        LockMouse(false);
        SceneManager.LoadScene(1);
    }

    public void SceneMenu()
    {
        GameController.Instance.SaveGameState();

        LockMouse(false);
        SceneManager.LoadScene(0);
    }

    public void LockMouse(bool toLock)
    {
        if (toLock)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
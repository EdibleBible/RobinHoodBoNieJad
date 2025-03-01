using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputEscMenu : MonoBehaviour
{
    public InputActionAsset globalInputActions;
    private InputAction escAction;
    public GameObject escapeMenu;

    private void OnEnable()
    {
        escAction = globalInputActions.FindAction("Esc");
        escAction.Enable();
        escAction.performed += ToggleEsc;
    }

    private void OnDisable()
    {
        escAction.performed -= ToggleEsc;
        escAction.Disable();
    }

    public void ToggleEsc(InputAction.CallbackContext context)
    {
        if (escapeMenu.activeInHierarchy)
        {
            escapeMenu.SetActive(false);
        }
        else
        {
            escapeMenu.SetActive(true);
        }
    }

    public void ToggleEsc()
    {
        if (escapeMenu.activeInHierarchy)
        {
            escapeMenu.SetActive(false);
        }
        else
        {
            escapeMenu.SetActive(true);
        }
    }

    public void SceneLobby()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(1);
    }

    public void SceneMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
}
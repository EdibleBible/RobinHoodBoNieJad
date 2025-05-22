using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputEscMenuInGame : MonoBehaviour
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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            escapeMenu.SetActive(true);
            Debug.Log("TUTAJ");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
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
        Debug.Log("TUTAJ");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(1);
    }

    public void SceneMenu()
    {
        Debug.Log("TUTAJ");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
}
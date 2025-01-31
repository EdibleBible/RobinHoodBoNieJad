using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonLobby : MonoBehaviour
{
    public void Open(GameObject lobbyObject)
    {
        lobbyObject.SetActive(true);
    }
    public void Close(GameObject lobbyObject)
    {
        lobbyObject.SetActive(false);
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(2);
    }
}

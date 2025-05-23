using UnityEngine;

public class MainMenuStartController : MonoBehaviour
{
    public void StartNewGame()
    {
        GameController.Instance.StartNewGame();
    }

    public void LoadGame()
    {
        GameController.Instance.LoadGame();
    }
}

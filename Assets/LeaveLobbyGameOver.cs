using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveLobbyGameOver : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameController.Instance.LeaveLobbyGameOver();
        }
    }
}

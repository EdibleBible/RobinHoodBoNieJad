using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputKeybinds : MonoBehaviour
{
    public KeyCode testKey;

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            this.gameObject.GetComponent<InputPlayerWalk>().stats.levelSuccess = true;
            SceneManager.LoadScene(0);
        }
    }
}

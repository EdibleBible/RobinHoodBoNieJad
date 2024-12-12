using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuChangeScene : MonoBehaviour
{
    public void SwitchScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}

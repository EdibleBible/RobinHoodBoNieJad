using Unity.VisualScripting;
using UnityEngine;

public class InputKeybinds : MonoBehaviour
{
    public KeyCode testKey;

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            Debug.Log("dupa");
        }
    }
}

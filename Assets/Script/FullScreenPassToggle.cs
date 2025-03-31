using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullScreenPassToggle : MonoBehaviour
{
    public ScriptableRendererFeature fullScreenPassFeature; // Przypisz w Inspectorze
    
    public float aaa;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFullScreenPass();
        }
    }

    void ToggleFullScreenPass()
    {
        if (fullScreenPassFeature != null)
        {
            fullScreenPassFeature.SetActive(!fullScreenPassFeature.isActive);
            Debug.Log("FullScreenPassRendererFeature: " + fullScreenPassFeature.isActive);
        }
    }



    
}
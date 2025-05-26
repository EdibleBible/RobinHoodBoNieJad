using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TorchUIHandler : MonoBehaviour
{
    [FormerlySerializedAs("staminaSlider")] [SerializeField]
    private List<Slider> torchSliders;
    
    public void UpdateFuelBar(Component sender, object data)
    {
        Debug.Log("Updating fuel bar");
        if (data is (float newFuel, float currentFuel) && sender is PlayerTorchSystem staminaSystem)
        {
            Debug.Log("Updating fuel bar2");

            foreach (var slider in torchSliders)
            {
                slider.maxValue = newFuel;
                slider.value = currentFuel;
            }
        }
    }

    public void ResizeFuelBar(Component sender, object data)
    {
        /*Debug.Log("Updating fuel bar3");

        if (data is (float newMaxFuel, float oldMaxFuel) && sender is PlayerTorchSystem staminaSystem)
        {
            Debug.Log("Updating fuel bar4");

            float scaleFactor = newMaxFuel / oldMaxFuel;
            float newWidth = Mathf.Max(originalWidth * scaleFactor, minBarWidth); // Zapewnienie minimalnej szerokości
            staminaBar.sizeDelta = new Vector2(newWidth, staminaBar.sizeDelta.y);
            originalWidth = newWidth;
        }*/
    }
}
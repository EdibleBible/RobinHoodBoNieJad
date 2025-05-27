using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorchUIHandler : MonoBehaviour
{
    [SerializeField] private List<Slider> staminaSliders;
    [SerializeField] private RectTransform staminaBar;
    [SerializeField] private float minBarWidth = 180f; // Minimalna szerokość paska staminy
    private float originalWidth;

    /*void Start()
    {
        originalWidth = staminaBar.sizeDelta.x;
    }*/

    public void UpdateFuelBar(Component sender, object data)
    {
        Debug.Log("Updating fuel bar");
        if (data is (float newFuel, float currentFuel) && sender is PlayerTorchSystem staminaSystem)
        {
            foreach (var staminaSlider in staminaSliders)
            {
                staminaSlider.maxValue = newFuel;
                staminaSlider.value = currentFuel;
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
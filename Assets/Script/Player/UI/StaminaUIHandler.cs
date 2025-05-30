using UnityEngine;
using UnityEngine.UI;

public class StaminaUIHandler : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider;
    

    public void UpdateStaminaBar(Component sender, object data)
    {
        if (data is (float maxStamina, float currentStamina) && sender is PlayerStaminaSystem staminaSystem)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
    }

    public void ResizeStaminaBar(Component sender, object data)
    {
        /*if (data is (float newMaxStamina, float oldMaxStamina) && sender is PlayerStaminaSystem staminaSystem)
        {
            float scaleFactor = newMaxStamina / oldMaxStamina;
            float newWidth = Mathf.Max(originalWidth * scaleFactor, minBarWidth);
            staminaBar.sizeDelta = new Vector2(newWidth, staminaBar.sizeDelta.y);
            originalWidth = newWidth;
        }*/
    }
}
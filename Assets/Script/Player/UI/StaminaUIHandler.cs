using UnityEngine;
using UnityEngine.UI;

public class StaminaUIHandler : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private RectTransform staminaBar;
    [SerializeField] private float minBarWidth = 180f; // Minimalna szerokość paska staminy
    private float originalWidth;

    void Start()
    {
        originalWidth = staminaBar.sizeDelta.x;
    }

    public void UpdateStaminaBar(Component sender, object data)
    {
        if (data is (float maxStamina, float currentStamina) && sender is PlayerStaminaSystem staminaSystem)
        {
            Debug.Log(staminaSlider.value);
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
        else
        {
            Debug.LogWarning("Stamina not set");
        }
    }

    public void ResizeStaminaBar(Component sender, object data)
    {
        if (data is (float newMaxStamina, float oldMaxStamina) && sender is PlayerStaminaSystem staminaSystem)
        {
            float scaleFactor = newMaxStamina / oldMaxStamina;
            float newWidth = Mathf.Max(originalWidth * scaleFactor, minBarWidth); // Zapewnienie minimalnej szerokości
            staminaBar.sizeDelta = new Vector2(newWidth, staminaBar.sizeDelta.y);
            originalWidth = newWidth;

        }
        else
        {
            Debug.LogWarning("Stamina to resize not set");
        }
    }
}
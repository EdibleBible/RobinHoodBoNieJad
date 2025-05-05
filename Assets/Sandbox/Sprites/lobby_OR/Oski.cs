using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Oski : MonoBehaviour
{
    public Color[] colors;
    public float fadeDuration = 2f;
    public float waitBetweenFades = 1f;

    public Volume postProcessVolume;
    private ColorAdjustments colorAdjustments;
    private int currentColorIndex = 0;

    private void Start()
    {
        if (postProcessVolume.profile.TryGet(out colorAdjustments))
        {
            StartColorCycle();
            Debug.Log("Color cycle started");
        }
        else
        {
            Debug.LogError("ColorGrading not found in PostProcessVolume!");
        }
    }

    void StartColorCycle()
    {
        if (colors.Length == 0) return;

        DOTween.To(
            () => colorAdjustments.colorFilter.value,
            x => colorAdjustments.colorFilter.value = x,
            colors[currentColorIndex],
            fadeDuration
        ).OnComplete(() =>
        {
            currentColorIndex = (currentColorIndex + 1) % colors.Length;
            DOVirtual.DelayedCall(waitBetweenFades, StartColorCycle);
        });
    }
}
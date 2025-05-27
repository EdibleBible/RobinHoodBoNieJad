using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SpecialSenseUICotnroller : MonoBehaviour
{
    public Slider specialSenseSlider;
    public TextMeshProUGUI specialSenseCountText;

    public void UpdateSpecialSenseCount(Component sender, object data)
    {
        if (sender is SpecialSenseController && data is int value)
        {
            specialSenseCountText.text = "X" + value.ToString();
        }
    }

    public void UpdateSpecialSenseBar(Component sender, object data)
    {
        if (sender is SpecialSenseController && data is float value)
        {
            specialSenseSlider.value = value;
        }
    }
}
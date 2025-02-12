using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuHotbarEntry : MonoBehaviour
{
    public Image selector;
    public Image image;
    public TMP_Text text;

    public void SwitchSelector(bool toSwitch)
    {
        selector.enabled = toSwitch;
    }
}

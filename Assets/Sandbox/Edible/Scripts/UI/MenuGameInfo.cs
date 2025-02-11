using TMPro;
using UnityEngine;

public class MenuGameInfo : MonoBehaviour
{
    void Start()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = "Robbin Graves v." + Application.version.ToString() + " © 2025 ProblemFix Games";
    }
}

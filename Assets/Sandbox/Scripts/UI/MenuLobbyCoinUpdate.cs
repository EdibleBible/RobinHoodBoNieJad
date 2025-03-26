using TMPro;
using UnityEngine;

public class MenuLobbyCoinUpdate : MonoBehaviour
{
    public SOStats SOStats;
    public TMP_Text text;

    void Update()
    {
        text.text = SOStats.scoreTotal.ToString();
    }
}

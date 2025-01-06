using TMPro;
using UnityEngine;

public class MenuDisplay : MonoBehaviour
{
    public SOStats stats;
    public TextMeshProUGUI tmp;

    public void UpdateScoreTotal()
    {
        tmp.text = stats.scoreTotal.ToString();
    }

    private void Start()
    {
        UpdateScoreTotal();
    }
}

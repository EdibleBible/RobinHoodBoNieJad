using TMPro;
using UnityEngine;

public class MenuDisplay : MonoBehaviour
{
    public SOInventory inventory;
    public TextMeshProUGUI scoreDisplay;

    public void UpdateScoreTotal()
    {
        scoreDisplay.text = inventory.playerScore.ToString();
    }

    private void Start()
    {
        UpdateScoreTotal();
    }
}

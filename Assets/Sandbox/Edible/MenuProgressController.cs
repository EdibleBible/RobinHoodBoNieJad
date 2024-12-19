using UnityEngine;

public class MenuProgressController : MonoBehaviour
{
    public SOStats stats;
    public MenuDisplay scoreDisplay;

    private void Awake()
    {
        if (stats.levelSuccess)
        {
            stats.scoreTotal += stats.scoreLevel;
        }
        stats.scoreLevel = 0;
        stats.levelSuccess = false;
        scoreDisplay.UpdateScoreTotal();
    }

    public void ResetProgressAll()
    {
        stats.scoreLevel = 0;
        stats.levelSuccess = false;
        stats.scoreTotal = 0;
        scoreDisplay.UpdateScoreTotal();
    }
}

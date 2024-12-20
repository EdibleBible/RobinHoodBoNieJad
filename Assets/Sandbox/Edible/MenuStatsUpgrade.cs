using UnityEngine;

public class MenuStatsUpgrade : MonoBehaviour
{
    public SOStats stats;

    public void UpgradeInventory()
    {
        stats.inventorySize = 4;
    }
    public void ResetInventory()
    {
        stats.inventorySize = 3;
    }
}

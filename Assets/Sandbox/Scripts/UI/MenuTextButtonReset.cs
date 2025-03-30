using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTextButtonReset : MonoBehaviour
{
    public SOInventory inventory;
    public SOStore store;
    public GameObject storeBackpack;
    public SOStats stats;

    public void Reset()
    {
        inventory.ItemsInInventory.Clear();
        inventory.CollectedItemTypes.Clear();
        inventory.CurrInvenoryScore = 0;
        inventory.BaseInventorySize = 3;
        inventory.CalculateItemsSlotsCount();
        store.storeItems.Clear();
        SceneManager.LoadScene(0);
        stats.playerSpeed = 1;
        stats.playerRotationSpeed = 360;
        stats.scoreLevel = 0;
        stats.levelSuccess = false;
        stats.scoreTotal = 0;
        stats.lobbyVisit = 1;
        stats.taxPaid = false;
        stats.inventorySize = 3;
    }
}

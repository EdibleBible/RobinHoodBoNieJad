using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTextButtonReset : MonoBehaviour
{
    public SOInventory inventory;
    public SOStore store;
    public GameObject storeBackpack;

    public void Reset()
    {
        inventory.ItemsInInventory.Clear();
        inventory.CollectedItemTypes.Clear();
        inventory.CurrInvenoryScore = 0;
        inventory.BaseInventorySize = 3;
        inventory.CalculateItemsSlotsCount();
        store.storeItems.Clear();
        store.storeItems.Add(storeBackpack.GetComponent<ItemModifier>());
        SceneManager.LoadScene(0);
    }
}

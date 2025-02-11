using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuCollectionEntry : MonoBehaviour
{
    public ItemType itemType;
    public Image icon;
    public Button button;
    public TMP_Text buttonText;
    public SOInventory inventory;

    private void OnEnable()
    {
        if (inventory.CollectedItemTypes.Contains(itemType))
        {
            buttonText.text = "Deposited";
            button.interactable = false;
            icon.color = Color.white;
            return;
        }
        foreach (ItemData item in inventory.ItemsInInventory)
        {
            if (item.ItemType == itemType)
            {
                return;
            }
        }
        buttonText.text = "Missing";
        button.interactable = false;
    }

    public void Deposit()
    {
        inventory.CollectedItemTypes.Add(itemType);
        buttonText.text = "Deposited";
        button.interactable = false;
        icon.color = Color.white;
    }
}

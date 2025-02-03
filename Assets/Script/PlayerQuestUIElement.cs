using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerQuestUIElement : MonoBehaviour
{
    public ItemType selectedItem;

    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private TextMeshProUGUI amountText;

    public void Setup(KeyValuePair<ItemType, QuestAmountData> item)
    {
        selectedItem = item.Key;
        if (questText != null)
            questText.text = item.Key.ToString();
        if(amountText != null)
            amountText.text = $"{item.Value.CurrentAmount}/{item.Value.RequiredAmount}";
    }

    public void UpdateValue(QuestAmountData amountData)
    {
        amountText.text = $"{amountData.CurrentAmount}/{amountData.RequiredAmount}";
    }
}
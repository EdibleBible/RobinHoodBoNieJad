using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerQuestUIElement : MonoBehaviour
{
    public ItemType selectedItem;
    public ItemsTranslator itemsTranslator;

    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private TextMeshProUGUI amountText;

    public void Setup(KeyValuePair<ItemType, QuestAmountData> item)
    {
        selectedItem = item.Key;
        if (questText != null)
            questText.text = itemsTranslator.itemTypeDictionary.Where(x => x.Key == item.Key).FirstOrDefault().Value;
        if(amountText != null)
            amountText.text = $"{item.Value.CurrentAmount}/{item.Value.RequiredAmount}";
    }

    public void UpdateValue(QuestAmountData amountData)
    {
        amountText.text = $"{amountData.CurrentAmount}/{amountData.RequiredAmount}";
    }
}


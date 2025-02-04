using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerQuestSystemUI : MonoBehaviour
{
    [SerializeField] private GameObject uiElementPrefab;
    [SerializeField] private Transform uiContainer;

    private SOPlayerQuest currShowedQuest;
    private List<PlayerQuestUIElement> playerQuestUIElements = new List<PlayerQuestUIElement>();

    public void SetupQuest(Component sender, object data)
    {
        if (sender is PlayerQuestSystem && data is SOPlayerQuest quest)
        {
            currShowedQuest = quest;
            foreach (var item in quest.RequireItems)
            {
                GameObject obj = Instantiate(uiElementPrefab, uiContainer);
                PlayerQuestUIElement playerQuestUIElement = obj.GetComponent<PlayerQuestUIElement>();
                playerQuestUIElements.Add(playerQuestUIElement);
                playerQuestUIElement.Setup(item);
            }
        }
    }
    
    public void UpdateValue(Component sender, object data)
    {
        if (data is (ItemType type, QuestAmountData amountData))
        {
            var selectedElement = playerQuestUIElements.Where(x => x.selectedItem == type).FirstOrDefault();
            if (selectedElement == null)
            {
                Debug.Log("No item to update");
            }
            else
            {
                selectedElement.UpdateValue(amountData);
            }
        }
    }
}
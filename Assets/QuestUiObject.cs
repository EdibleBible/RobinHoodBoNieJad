using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestUiObject : MonoBehaviour
{
    public SOPlayerQuest SelectedQuest;
    public TextMeshProUGUI QuestText;
    public Button QuestButton;

    public GameEvent ShowQuestDescriptionUIEvent;

    public void Setup(SOPlayerQuest selectedQuest, UnityAction buttonAction)
    {
        SelectedQuest = selectedQuest;
        QuestText.text = selectedQuest.ShortDescription;
        
        QuestButton.onClick.RemoveAllListeners();
        QuestButton.onClick.AddListener(ShowQuestDescription);
        QuestButton.onClick.AddListener(buttonAction);
    }

    public void ShowQuestDescription()
    {
        GameController.Instance.AllPlayerQuest.SelectQuest(SelectedQuest);
    }
}
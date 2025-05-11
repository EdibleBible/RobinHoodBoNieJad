using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestUiObject : MonoBehaviour
{
    public SOPlayerQuest SelectedQuest;
    public TextMeshProUGUI QuestNameText;
    public TextMeshProUGUI QuestDescriptionText;

    public Button QuestButton;

    public GameEvent ShowQuestDescriptionUIEvent;

    public void Setup(SOPlayerQuest selectedQuest, UnityAction buttonAction)
    {
        SelectedQuest = selectedQuest;
        if (QuestNameText != null)
            QuestNameText.text = selectedQuest.QuestName;
        
        if (QuestDescriptionText != null)
            QuestDescriptionText.text = selectedQuest.GetDifficultyAsText(selectedQuest.Difficulty);

        QuestButton.onClick.RemoveAllListeners();
        QuestButton.onClick.AddListener(ShowQuestDescription);
        QuestButton.onClick.AddListener(buttonAction);
    }

    public void ShowQuestDescription()
    {
        GameController.Instance.AllPlayerQuest.SelectQuest(SelectedQuest);
    }
}
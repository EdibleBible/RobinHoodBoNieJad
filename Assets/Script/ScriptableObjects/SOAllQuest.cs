using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create SOAllQuest", fileName = "SOAllQuest", order = 0)]
public class SOAllQuest : ScriptableObject
{
    public int AllQuestsCount;

    public SOPlayerQuest CurrentSelectedQuest;
    public List<SOPlayerQuest> randomizedQuests = new List<SOPlayerQuest>();

    public void RandomizeAllQuests()
    {
        randomizedQuests.Clear();
        for (int i = 0; i < AllQuestsCount; i++)
        {
            SOPlayerQuest quest = ScriptableObject.CreateInstance<SOPlayerQuest>();
            randomizedQuests.Add(quest);
            quest.RandomizeQuest(QuestDifficulty.easy);
        }
    }

    public void RandomizeSelectedQuest(QuestDifficulty currentQuestDifficulty, bool questCompleted, SOPlayerQuest quest)
    {
        QuestDifficulty newDifficulty = QuestDifficulty.easy;
        int randomRange = Random.Range(1, 11);

        if (questCompleted)
        {
            if (randomRange >= 6 && currentQuestDifficulty != QuestDifficulty.VeryHard)
            {
                newDifficulty = currentQuestDifficulty + 1;
            }
            else
            {
                newDifficulty = currentQuestDifficulty;
            }
        }
        else
        {
            if (randomRange >= 7 && currentQuestDifficulty != QuestDifficulty.easy)
            {
                newDifficulty = currentQuestDifficulty - 1;
            }
            else
            {
                newDifficulty = currentQuestDifficulty;
            }
        }

        randomizedQuests[randomizedQuests.IndexOf(quest)].RandomizeQuest(newDifficulty);
    }


    public void LoadAllQuest(int allQuestsCount, int selectedQuestsindex, List<QuestSaveData> randomizedQuest)
    {
        AllQuestsCount = allQuestsCount;
        randomizedQuests.Clear();

        foreach (var questData in randomizedQuest)
        {
            SOPlayerQuest quest = ScriptableObject.CreateInstance<SOPlayerQuest>();
            quest.QuestName = questData.QuestName;
            quest.Difficulty = (QuestDifficulty)questData.QuestDifficulty;
            quest.Description = questData.Description;
            quest.ShortDescription = questData.ShortDescription;
            randomizedQuests.Add(quest);
        }

        if (selectedQuestsindex > -1)
            CurrentSelectedQuest = randomizedQuests[selectedQuestsindex];
    }

    public void SelectQuest(SOPlayerQuest quest)
    {
        CurrentSelectedQuest = quest;
    }
}
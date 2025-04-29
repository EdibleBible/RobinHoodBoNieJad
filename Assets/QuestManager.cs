using System;
using UnityEngine;
using UnityEngine.Serialization;

public class QuestManager : MonoBehaviour
{
    [FormerlySerializedAs("CurrentQuestSO")] public SOAllQuest currentQuestSo;

    private void Awake()
    {
        if (currentQuestSo == null)
        {
            currentQuestSo = ScriptableObject.CreateInstance<SOAllQuest>();
            currentQuestSo.RandomizeAllQuests();
        }
    }

    public void SaveAllQuests()
    {
        
    }

    public void LoadAllQuests()
    {
        
    }
}
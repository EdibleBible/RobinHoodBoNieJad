using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SOPlayerQuest", menuName = "Scriptable Objects/SOPlayerQuest")]
public class SOPlayerQuest : ScriptableObject
{
    public QuestDifficulty Difficulty;
    public string Description;
    public string ShortDescription;

    public SerializedDictionary<ItemType, QuestAmountData> RequireItems =
        new SerializedDictionary<ItemType, QuestAmountData>();

    public SerializedDictionary<ItemType, int> SpawnedItems = new SerializedDictionary<ItemType, int>();

    public void Reset()
    {
        foreach (var item in RequireItems)
        {
            item.Value.CurrentAmount = 0;
        }
    }

    public bool IsQuestComplete()
    {
        foreach (var item in RequireItems)
        {
            if (item.Value.CurrentAmount < item.Value.RequiredAmount)
            {
                return false;
            }
        }

        return true;
    }

    public void RandomizeQuest(QuestDifficulty difficulty)
    {
        Difficulty = difficulty;
        RequireItems.Clear();

        ShortDescription = $"New Quest {Difficulty}";

        
        int itemAmount = 0;
        int itemRequiredAmountMin = 0;
        int itemRequiredAmountMax = 0;
        List<ItemType> avaibleCollection = ItemTypeHelper.GetCollectibles();

        switch (Difficulty)
        {
            case QuestDifficulty.easy:
                itemAmount = Random.Range(1, 3);
                itemRequiredAmountMax = 3;
                itemRequiredAmountMin = 1;
                break;

            case QuestDifficulty.medium:
                itemAmount = Random.Range(3, 5);
                itemRequiredAmountMax = 5;
                itemRequiredAmountMin = 1;
                break;

            case QuestDifficulty.Hard:
                itemAmount = Random.Range(5, 7);
                itemRequiredAmountMax = 5;
                itemRequiredAmountMin = 3;
                break;

            case QuestDifficulty.VeryHard:
                itemAmount = Random.Range(7, 9);
                itemRequiredAmountMax = 7;
                itemRequiredAmountMin = 3;
                break;
        }

        for (int i = 0; i < itemAmount; i++)
        {
            if (avaibleCollection.Count <= 0)
                break;

            ItemType itemType = avaibleCollection[Random.Range(0, avaibleCollection.Count)];
            avaibleCollection.Remove(itemType);

            RequireItems.Add(itemType,
                new QuestAmountData(0, Random.Range(itemRequiredAmountMin, itemRequiredAmountMax)));
        }
        
        
        Description = $"New Quest {Difficulty}";

        foreach (var item in RequireItems)
        {
            Description += $"\n{item.Key} Amount: {item.Value.RequiredAmount}\n";
        }
    }

    public void ResetSpawnedItem()
    {
        SpawnedItems.Clear();
    }
}

[Serializable]
public class QuestAmountData
{
    public int CurrentAmount;
    public int RequiredAmount;

    public QuestAmountData(int currentAmount, int requiredAmount)
    {
        CurrentAmount = currentAmount;
        RequiredAmount = requiredAmount;
    }
}

public enum QuestDifficulty
{
    easy = 1,
    medium = 2,
    Hard = 3,
    VeryHard = 4
}
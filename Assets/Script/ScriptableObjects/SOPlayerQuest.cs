using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "SOPlayerQuest", menuName = "Scriptable Objects/SOPlayerQuest")]
public class SOPlayerQuest : ScriptableObject
{
    public SerializedDictionary<ItemType, QuestAmountData> RequireItems;

    public void Reset()
    {
        foreach (var item in RequireItems)
        {
            item.Value.CurrentAmount = 0;
        }
    }
}

[Serializable]
public class QuestAmountData
{
    public int CurrentAmount;
    public int RequiredAmount;
}
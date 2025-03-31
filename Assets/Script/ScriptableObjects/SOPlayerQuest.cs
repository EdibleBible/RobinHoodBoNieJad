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

    public bool IsQuestComplete()
    {
        foreach (var item in RequireItems)
        {
            if (item.Value.CurrentAmount < item.Value.RequiredAmount)
            {
                return false; // Jeśli którykolwiek przedmiot nie spełnia wymagań, zadanie nie jest ukończone
            }
        }
        return true; // Wszystkie przedmioty spełniają wymagania
    }
}

[Serializable]
public class QuestAmountData
{
    public int CurrentAmount;
    public int RequiredAmount;
}
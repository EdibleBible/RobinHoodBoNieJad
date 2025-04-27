using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerQuestSystem : MonoBehaviour
{
    [SerializeField] private SOAllQuest allQuest;
    [SerializeField] private GameEvent SetUpQuestEvent;
    [SerializeField] private GameEvent UpdateQuestValueEvent;

    [SerializeField] private bool Debug;

    private void Awake()
    {
        if (allQuest == null)
            return;

        if (Debug)
        {
            allQuest.RandomizeAllQuests();
            allQuest.CurrentSelectedQuest = allQuest.randomizedQuests[0];

            foreach (var VARIABLE in allQuest.CurrentSelectedQuest.RequireItems)
            {
                UnityEngine.Debug.Log($"item name {VARIABLE.Key} amount {VARIABLE.Value}");
            }
        }
        else
        {
            if (GameController.Instance != null)
            {
                allQuest = GameController.Instance.AllPlayerQuest;
            }
        }
        
        allQuest.CurrentSelectedQuest.Reset();
    }

    private void Start()
    {
        SetUpQuestEvent?.Raise(this, allQuest.CurrentSelectedQuest);
    }

    public void SetUpQuest(SOAllQuest quest)
    {
        allQuest = quest;
        allQuest.CurrentSelectedQuest.Reset();
        SetUpQuestEvent?.Raise(this, allQuest.CurrentSelectedQuest);
    }

    public void AddItemToQuest(Component sender, object data)
    {
        if (data is ItemData itemBase && sender is PlayerBase playerBase)
        {
            if (allQuest.CurrentSelectedQuest.RequireItems.ContainsKey(itemBase.ItemType))
            {
                allQuest.CurrentSelectedQuest.RequireItems[itemBase.ItemType].CurrentAmount++;
                var value = allQuest.CurrentSelectedQuest.RequireItems[itemBase.ItemType];
                UpdateQuestValueEvent?.Raise(this,
                    (itemBase.ItemType, allQuest.CurrentSelectedQuest.RequireItems[itemBase.ItemType]));
            }
        }
    }

    public void RemoveItemFromQuest(Component sender, object data)
    {
        if (data is ItemData itemBase)
        {
            if (allQuest.CurrentSelectedQuest.RequireItems.ContainsKey(itemBase.ItemType))
            {
                allQuest.CurrentSelectedQuest.RequireItems[itemBase.ItemType].CurrentAmount--;
                UpdateQuestValueEvent?.Raise(this,
                    (itemBase.ItemType, allQuest.CurrentSelectedQuest.RequireItems[itemBase.ItemType]));
            }
        }
    }

    public bool IsQuestComplete()
    {
        return allQuest.CurrentSelectedQuest.RequireItems.All(item =>
            item.Value.CurrentAmount >= item.Value.RequiredAmount);
    }
}
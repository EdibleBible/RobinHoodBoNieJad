using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerQuestSystem : MonoBehaviour
{
    [SerializeField] private SOPlayerQuest playerCurrentQuests;
    [SerializeField] private GameEvent SetUpQuestEvent;
    [SerializeField] private GameEvent UpdateQuestValueEvent;

    private void Awake()
    {
        playerCurrentQuests.Reset();
    }

    private void Start()
    {
        SetUpQuestEvent?.Raise(this, playerCurrentQuests);
    }

    public void AddItemToQuest(Component sender, object data)
    {
        if (data is ItemData itemBase && sender is PlayerBase playerBase)
        {

            if (playerCurrentQuests.RequireItems.ContainsKey(itemBase.ItemType))
            {
                playerCurrentQuests.RequireItems[itemBase.ItemType].CurrentAmount++;
                var value = playerCurrentQuests.RequireItems[itemBase.ItemType];
                UpdateQuestValueEvent?.Raise(this, (itemBase.ItemType ,playerCurrentQuests.RequireItems[itemBase.ItemType]));
            }
        }
    }

    public void RemoveItemFromQuest(Component sender, object data)
    {
        if (data is ItemData itemBase)
        {
            if (playerCurrentQuests.RequireItems.ContainsKey(itemBase.ItemType))
            {
                playerCurrentQuests.RequireItems[itemBase.ItemType].CurrentAmount--;
                UpdateQuestValueEvent?.Raise(this, (itemBase.ItemType ,playerCurrentQuests.RequireItems[itemBase.ItemType]));
            }
        }
    }
}
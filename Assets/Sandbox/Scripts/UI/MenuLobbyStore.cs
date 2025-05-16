using System;
using System.Collections.Generic;
using System.Linq;
using Script.ScriptableObjects;
using UnityEngine;

public class MenuLobbyStore : MonoBehaviour
{
    public SOPlayerStatsController PlayerStatsController;
    public List<LobbyStoreUiElement> lobbyStoreUiElements = new List<LobbyStoreUiElement>();
    public List<E_ModifiersType> modifiersTypes = new List<E_ModifiersType>();
    public int MaxLevel = 5;
    private void OnEnable()
    {
        for (int i = 0; i < modifiersTypes.Count; i++)
        {
            if (PlayerStatsController.PlayerBaseModifiers.Any(x => x.ModifiersType == modifiersTypes[i]))
            {
                lobbyStoreUiElements[i].SetUp(PlayerStatsController.PlayerBaseModifiers.Where(x => x.ModifiersType == modifiersTypes[i]).FirstOrDefault(),PlayerStatsController,MaxLevel);
            }
            else
            {
                if (PlayerStatsController.PlayerStats.Any(x => x.ModifiersType == modifiersTypes[i]))
                {
                    lobbyStoreUiElements[i].SetUp(PlayerStatsController.PlayerStats.Where(x => x.ModifiersType == modifiersTypes[i]).FirstOrDefault(),PlayerStatsController,MaxLevel);
                }
            }
        }
    }
}
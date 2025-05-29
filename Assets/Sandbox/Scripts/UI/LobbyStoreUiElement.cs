using System.Linq;
using Script.ScriptableObjects;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class LobbyStoreUiElement : MonoBehaviour
{
    public TextMeshProUGUI StatName;
    public TextMeshProUGUI StatCurrLevel;
    public TextMeshProUGUI StatCurrCost;

    public Image StatIcon;
    public Button StatButton;

    public GameEvent ReloadAllButtons;
    
    public UpgradesTranslator upgradesTranslator;

    public void SetUp(StatsModifiers stat, SOPlayerStatsController playerStatsController, int maxLevel,
        SOInventory playerInventory,SOStats stats)
    {
        if (StatName != null)
        {
            StatName.text = upgradesTranslator.StatText.Where(x => x.Key == stat.ModifiersType).FirstOrDefault().Value;
        }

        if (StatCurrLevel != null)
        {
            if (stat.CurrLevel < maxLevel)
            {
                StatCurrLevel.text = "Level: " + stat.CurrLevel.ToString() + "/" + maxLevel.ToString();
            }
            else
            {
                StatCurrLevel.text = "MAX LEVEL";
            }
        }

        if (StatCurrCost != null)
        {
            if (stat.CurrLevel < maxLevel)
            {
                StatCurrCost.text = "Cost: " + stat.GetCurrentCost();
            }
            else
            {
                StatCurrCost.gameObject.SetActive(false);
            }
        }

        if (StatIcon != null)
        {
            StatIcon.color = new Color(255f, 255f, 255f, 255f);
            StatIcon.sprite = stat.Icon;
        }

        if (StatButton != null)
        {
            if (stat.CurrLevel < maxLevel)
            {
                StatButton.gameObject.SetActive(true);
                StatButton.onClick.RemoveAllListeners();
            }
            else
            {
                StatButton.gameObject.SetActive(false);
            }

            if (stat.GetCurrentCost() > stats.scoreTotal)
            {
                StatButton.interactable = false;
                return;
            }
            else
            {
                StatButton.interactable = true;
            }


            if (stat.CurrLevel < maxLevel)
            {
                StatParameters parameters = new StatParameters();
                parameters.ModifierType = stat.ModifiersType;
                parameters.Additive = stat.statsToUpgrade.Additive;
                parameters.Multiplicative = stat.statsToUpgrade.Multiplicative;

                StatButton.onClick.AddListener(() =>
                {
                    ButButtonController(stat, playerStatsController, maxLevel, parameters, playerInventory,stats);
                });
            }
            else
            {
                StatButton.gameObject.SetActive(false);
            }
        }
    }

    private void ButButtonController(StatsModifiers stat, SOPlayerStatsController playerStatsController, int maxLevel,
        StatParameters parameters, SOInventory playerInventory,SOStats stats)
    {
        if (!CheckMoney(stat.GetCurrentCost(), stats))
        {
            return;
        }

        var newStat = playerStatsController.PlayerBaseModifiers
            .Where(x => x.ModifiersType == stat.ModifiersType).FirstOrDefault();
        if (newStat == null)
        {
            playerStatsController.UpgradePlayerBaseModifiers(parameters, stat.Icon, stat.CurrLevel,
                stat.UpgradeBaseCost);
            newStat = playerStatsController.PlayerBaseModifiers
                .Where(x => x.ModifiersType == stat.ModifiersType).FirstOrDefault();
        }

        else
        {
            playerStatsController.UpgradePlayerBaseModifiers(parameters, newStat.Icon, newStat.CurrLevel,
                newStat.UpgradeBaseCost);
        }

        SetUp(newStat, playerStatsController, maxLevel, playerInventory,stats);
        ReloadAllButtons.Raise(this,null);
    }

    public bool CheckMoney(int cost, SOStats stats)
    {
        if (cost > stats.scoreTotal)
        {
            return false;
        }
        else
        {
            stats.scoreTotal -= cost;
            return true;
        }
    }
}
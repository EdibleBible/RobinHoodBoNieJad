using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Script.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SOPlayerStats", menuName = "Scriptable Objects/SOPlayerStats")]
    public class SOPlayerStatsController : ScriptableObject
    {
        public string AllStatsPath;

        public List<StatsModifiers> PlayerStats = new List<StatsModifiers>();
        public List<StatsModifiers> PlayerBaseModifiers = new List<StatsModifiers>();

        public void LoadStats(string saveDataAllStatsPath, List<StatsModifierSaveData> saveDataPlayerStats,
            List<StatsModifierSaveData> saveDataPlayerBaseModifiers)
        {
            AllStatsPath = saveDataAllStatsPath;

            PlayerStats.Clear();
            foreach (var saveData in saveDataPlayerStats)
            {
                Sprite selectedSprite = LoadSpriteByName(saveData.SpriteName);

                StatsModifiers temp = new StatsModifiers((E_ModifiersType)saveData.ModifierType, saveData.Additive,
                    saveData.Multiplicative, selectedSprite, saveData.UpgradeBaseCost, saveData.CurrLevel,
                    saveData.UpgradeCurrCost, saveData.AdditiveToAdd, saveData.MultiplicativeToAdd);
                
                PlayerStats.Add(temp);
            }
            
            PlayerBaseModifiers.Clear();
            foreach (var saveData in saveDataPlayerBaseModifiers)
            {
                Sprite selectedSprite = LoadSpriteByName(saveData.SpriteName);

                StatsModifiers temp = new StatsModifiers((E_ModifiersType)saveData.ModifierType, saveData.Additive,
                    saveData.Multiplicative, selectedSprite, saveData.UpgradeBaseCost, saveData.CurrLevel,
                    saveData.UpgradeCurrCost, saveData.AdditiveToAdd, saveData.MultiplicativeToAdd);
                
                PlayerBaseModifiers.Add(temp);
            }
        }


        public Sprite LoadSpriteByName(string spriteName)
        {
            Sprite[] allSprites = Resources.LoadAll<Sprite>("Sprites");

            foreach (Sprite sprite in allSprites)
            {
                if (sprite.name == spriteName)
                {
                    return sprite;
                }
            }

            Debug.LogWarning($"Sprite '{spriteName}' not found in Resources/Sprites or its subfolders.");
            return null;
        }


        public StatsModifiers GetSOPlayerStats(E_ModifiersType modifiersType)
        {
            if (PlayerStats.Count == 0)
                return null;

            if (PlayerStats.Where(x => x.ModifiersType == modifiersType).ToList().Count == 0)
                return null;

            var tempolaryStatModifiers = PlayerStats.Where(x => x.ModifiersType == modifiersType).FirstOrDefault();
            tempolaryStatModifiers.Multiplicative = Mathf.Max(tempolaryStatModifiers.Multiplicative, 0.1f);
            return tempolaryStatModifiers;
        }

        public void SetModifier(float additiveValue, float multiplicativeValue, E_ModifiersType modifiersType)
        {
            if (PlayerStats.Count == 0)
                return;

            if (PlayerStats.Where(x => x.ModifiersType == modifiersType).ToList().Count == 0)
                return;

            var selectedStat = PlayerStats.Where(x => x.ModifiersType == modifiersType).FirstOrDefault();
            selectedStat.SetAdditive(additiveValue);
            selectedStat.SetMultiplicative(multiplicativeValue);
        }

        public void ChangeModifier(float additiveValue, float multiplicativeValue, E_ModifiersType modifiersType)
        {
            if (PlayerStats.Count == 0)
                return;

            if (PlayerStats.Where(x => x.ModifiersType == modifiersType).ToList().Count == 0)
                return;

            var selectedStat = PlayerStats.Where(x => x.ModifiersType == modifiersType).FirstOrDefault();
            selectedStat.ChangeAdditive(additiveValue);
            selectedStat.ChangeMultiplicative(multiplicativeValue);
        }

        public void ResetAllModifiers()
        {
            foreach (var stat in PlayerStats)
            {
                stat.Reset();
            }
        }

        public void ResetSelectedModifiers(E_ModifiersType modifiersType)
        {
            if (PlayerStats.Count == 0)
                return;

            if (PlayerStats.Where(x => x.ModifiersType == modifiersType).ToList().Count == 0)
                return;

            var selectedStat = PlayerStats.Where(x => x.ModifiersType == modifiersType).FirstOrDefault();
            selectedStat.Reset();
        }

        public void SetPlayerBaseModifiers()
        {
            ResetAllModifiers();
            foreach (var baseModifier in PlayerBaseModifiers)
            {
                if (!PlayerStats.Any(x => x.ModifiersType == baseModifier.ModifiersType))
                    continue;

                var selectedMofifier = PlayerStats.Where(x => x.ModifiersType == baseModifier.ModifiersType)
                    .FirstOrDefault();


                SetModifier(baseModifier.Additive + selectedMofifier.Additive,
                    baseModifier.Multiplicative + selectedMofifier.Multiplicative, baseModifier.ModifiersType);
            }
        }

        public void UpgradePlayerBaseModifiers(StatParameters modifier, Sprite sprite, int level, int baseCost)
        {
            Debug.Log("level: " + level);
            if (PlayerBaseModifiers.Any(x => x.ModifiersType == modifier.ModifierType))
            {
                var selectedModifier = PlayerBaseModifiers.Where(x => x.ModifiersType == modifier.ModifierType)
                    .FirstOrDefault();
                selectedModifier.ChangeAdditive(modifier.Additive);
                selectedModifier.ChangeMultiplicative(modifier.Multiplicative);
                selectedModifier.CurrLevel = level + 1;
            }
            else
            {
                StatsModifiers newBaseMod =
                    new StatsModifiers(modifier.ModifierType, modifier.Additive, modifier.Multiplicative);
                newBaseMod.statsToUpgrade.Multiplicative = modifier.Multiplicative;
                newBaseMod.statsToUpgrade.Additive = modifier.Additive;
                newBaseMod.CurrLevel = level + 1;
                newBaseMod.UpgradeBaseCost = baseCost;
                newBaseMod.Icon = sprite;
                PlayerBaseModifiers.Add(newBaseMod);
            }

            SetPlayerBaseModifiers();
        }

        public void ClearPlayerBaseModifiers()
        {
            PlayerBaseModifiers.Clear();
            SetPlayerBaseModifiers();
        }

        public void RemoveAllBaseModier()
        {
            PlayerBaseModifiers.Clear();
        }
    }
}
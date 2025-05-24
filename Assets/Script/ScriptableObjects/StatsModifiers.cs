using System;
using UnityEngine;

namespace Script.ScriptableObjects
{
    [Serializable]
    public class StatsModifiers 
    {
        public E_ModifiersType ModifiersType;
        public float Additive;
        public float Multiplicative;


        public Sprite Icon;
        public int UpgradeBaseCost;
        public int CurrLevel;
        public int UpgradeCurrCost;
        
        [Serializable]
        public class StatsToUpgrade
        {
            public float Additive;
            public float Multiplicative;
        }

        public StatsToUpgrade statsToUpgrade = new StatsToUpgrade();

        public StatsModifiers(E_ModifiersType modifiersType, float additive, float multiplicative)
        {
            ModifiersType = modifiersType;
            Additive = additive;
            Multiplicative = multiplicative;
        }


        public StatsModifiers(E_ModifiersType modifiersType, float additive, float multiplicative, Sprite icon, int upgradeBaseCost, int currLevel, int upgradeCurrCost, float upgradeAdditive, float upgradeMultiplicative)
        {
            ModifiersType = modifiersType;
            Additive = additive;
            Multiplicative = multiplicative;
            Icon = icon;
            UpgradeBaseCost = upgradeBaseCost;
            CurrLevel = currLevel;
            UpgradeCurrCost = upgradeCurrCost;
            
            statsToUpgrade = new StatsToUpgrade();
            statsToUpgrade.Additive = upgradeAdditive;
            statsToUpgrade.Multiplicative = upgradeMultiplicative;
        }

        public void Reset()
        {
            Additive = 0;
            Multiplicative = 1;
        }

        public void SetAdditive(float value)
        {
            Additive = value;
        }

        public void SetMultiplicative(float value)
        {
            Multiplicative = value;
        }

        public void ChangeAdditive(float value)
        {
            Additive += value;
        }

        public void ChangeMultiplicative(float value)
        {
            Multiplicative += value;
        }

        public int GetCurrentCost()
        {
            UpgradeCurrCost = CurrLevel * UpgradeBaseCost;
            return UpgradeCurrCost;
        }
    }
}
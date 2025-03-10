using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SOPlayerStats", menuName = "Scriptable Objects/SOPlayerStats")]
    public class SOPlayerStatsController : ScriptableObject
    {
        public List<SOStatsModifiers> PlayerStats = new List<SOStatsModifiers>();

        public SOStatsModifiers GetSOPlayerStats(E_ModifiersType modifiersType)
        {
            if (PlayerStats.Count == 0)
                return null;

            if (PlayerStats.Where(x => x.ModifiersType == modifiersType).ToList().Count == 0)
                return null;

            return PlayerStats.Where(x => x.ModifiersType == modifiersType).FirstOrDefault();
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
    }
}
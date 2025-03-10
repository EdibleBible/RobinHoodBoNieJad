using System;
using UnityEngine;

namespace Script.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SOPlayerStats", menuName = "Scriptable Objects/SOStat")]
    public class SOStatsModifiers : ScriptableObject
    {
        public E_ModifiersType ModifiersType;
        public float Additive;
        public float Multiplicative;


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
    }
}
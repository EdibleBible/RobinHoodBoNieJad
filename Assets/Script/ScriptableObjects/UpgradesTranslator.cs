using AYellowpaper.SerializedCollections;
using Script.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesTranslator", menuName = "Scriptable Objects/UpgradesTranslator")]
public class UpgradesTranslator : ScriptableObject
{
    public SerializedDictionary<E_ModifiersType,string> StatText = new SerializedDictionary<E_ModifiersType, string>();
}

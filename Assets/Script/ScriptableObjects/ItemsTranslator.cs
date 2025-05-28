using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsTranslator", menuName = "Scriptable Objects/ItemsTranslator")]
public class ItemsTranslator : ScriptableObject
{
    public SerializedDictionary<ItemType,string> itemTypeDictionary;
}

using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SO_ActionTextMap", menuName = "Input/ActionTextMap")]
public class SO_ActionTextMap : ScriptableObject
{
    public SerializedDictionary<InputActionReference,string> Inputs = new ();
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SOLevel", menuName = "Scriptable Objects/SOLevel")]
public class SOLevel : ScriptableObject
{
    public int LevelSeedInt;
    public uint LevelSeedUint;
}

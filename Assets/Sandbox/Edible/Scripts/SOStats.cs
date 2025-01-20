using UnityEngine;

[CreateAssetMenu(fileName = "SOStats", menuName = "Scriptable Objects/SOStats")]
public class SOStats : ScriptableObject
{
    public int inventorySize;
    public float playerSpeed;
    public float playerRotationSpeed;
    public int scoreLevel;
    public bool levelSuccess;
    public int scoreTotal;
}

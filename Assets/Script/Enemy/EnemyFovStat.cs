using UnityEngine;

[CreateAssetMenu(fileName = "EnemyFovStat", menuName = "Scriptable Objects/EnemyFovStat")]
public class EnemyFovStat : ScriptableObject
{
    public float ViewRadius;
    public float ViewAngle;
    public float FindingDelay;

    public LayerMask TargetLayer;
    public LayerMask ObstacleLayer;
}

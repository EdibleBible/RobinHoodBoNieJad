using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMoveStats", menuName = "Scriptable Objects/EnemyMoveStats")]
public class EnemyMoveStats : ScriptableObject
{
    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float AngularSpeed;
    public float DelayBettwenDistanceCheck;
    public float StoppingDistance;
    public float ChangeDestinationPointDistance;
    public float FindPointDistance;
}

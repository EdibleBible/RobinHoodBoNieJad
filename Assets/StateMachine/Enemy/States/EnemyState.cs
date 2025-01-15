using System;
using Unity.Behavior;

[BlackboardEnum]
public enum EnemyState
{
    Idle,
	Patroll,
	Chase,
	Allarmed,
	PatrollRandom
}

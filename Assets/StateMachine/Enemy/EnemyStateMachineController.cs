using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyStateMachineController : StateManager<E_EnemyState>
{
    private EnemyPatrollingState _enemyPatrollingState;
    private EnemyChasingState _enemyChasingState;
    
    [Header("Idle State")]
    [SerializeField] private EnemyMovementStats idleStats;
    
    [Header("Patrolling State")]
    [SerializeField] private EnemyMovementStats patrollingStats;
    [SerializeField] private EnemyFovStats fovPatrollingState;
    [SerializeField] private float findingPointDistance;
    
    [Header("Chase State")]
    [SerializeField] private EnemyMovementStats chasingStats; 
    
    private EnemyMovement enemyMovement;
    private FieldOfView fov;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        fov = GetComponent<FieldOfView>();
    }

    public override void Start()
    {
        _enemyPatrollingState = new EnemyPatrollingState(fov,fovPatrollingState,enemyMovement,patrollingStats,findingPointDistance);
        _enemyChasingState = new EnemyChasingState(fov,fovPatrollingState,enemyMovement,chasingStats);
        
        
        state.Add(E_EnemyState.Patrol,_enemyPatrollingState);
        state.Add(E_EnemyState.Chase, _enemyChasingState);
        
        currentState = _enemyPatrollingState;
        base.Start();
    }

    public override void Update()
    {
        Debug.Log(fov.GetVisibleTargets().Count);
        Debug.Log("State: " + currentState);
        base.Update();
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
    }
}

[Serializable]
public class EnemyMovementStats
{
    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float AngularSpeed;
    public float DelayBettwenDistanceCheck;
    public float StoppingDistance;
    public float ChangeDestinationPointDistance;
}

[Serializable]
public class EnemyFovStats
{
    public float ViewRadius;
    public float ViewAngle;
    public float FindingDelay;

    public LayerMask TargetLayer;
    public LayerMask ObstacleLayer;
}
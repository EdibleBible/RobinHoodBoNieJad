using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachineController : StateManager<E_EnemyState>
{
    [SerializeField] private EnemyIdleState _enemyIdleState;
    [SerializeField] private EnemyPatrolState _enemyPatrolState;

    [Header("Patrol State")] 
    [SerializeField] private float patrolSpeed;
    [SerializeField] private List<Vector3> patrolPoints;
    
    public override void Start()
    {
        _enemyIdleState = new EnemyIdleState(E_EnemyState.Idle);
        _enemyPatrolState = new EnemyPatrolState(E_EnemyState.Patrol);

        state.Add(E_EnemyState.Idle, _enemyIdleState);
        state.Add(E_EnemyState.Patrol, _enemyPatrolState);

        currentState = _enemyIdleState;

        base.Start();
    }

    public override void Update()
    {
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
public class EnemyPatrolState : BaseState<E_EnemyState>
{
    public EnemyPatrolState(E_EnemyState key) : base(key)
    {
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }

    public override E_EnemyState GetNextState()
    {
        return stateKey;
    }

    public override void OnTriggerEnterState(Collider other)
    {
    }

    public override void OnTriggerStayState(Collider other)
    {
    }

    public override void OnTriggerExitState(Collider other)
    {
    }
}

public class EnemyIdleState : BaseState<E_EnemyState>
{
    private bool playerInRange;
    private bool hearOtherEnemyChase;
    public EnemyIdleState(E_EnemyState key) : base(key)
    {
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }

    public override E_EnemyState GetNextState()
    {
        if (hearOtherEnemyChase)
        {
            return E_EnemyState.FollowChase;
        }

        if (playerInRange)
        {
            return E_EnemyState.Chase;
        }
        return E_EnemyState.Patrol;
    }

    public override void OnTriggerEnterState(Collider other)
    {
    }

    public override void OnTriggerStayState(Collider other)
    {
    }

    public override void OnTriggerExitState(Collider other)
    {
    }
}
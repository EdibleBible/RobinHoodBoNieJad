using System;
using UnityEngine;

public class EnemyAlarmedState : BaseState<E_EnemyState>
{
    private EnemyMovement movement;
    private EnemyMovementStats movementStats;

    private FieldOfView fov;
    private EnemyFovStats fovStats;

    private EnemyAlarmedStats alarmedStats;

    private bool isSmallPatrolling;
    private int patrollCounter = 0;

    public EnemyAlarmedState(EnemyMovement _movement, EnemyMovementStats _movementStats, FieldOfView _fov,
        EnemyFovStats _fovStats, EnemyAlarmedStats _alarmedStats) : base(E_EnemyState.Alarmed)
    {
        movement = _movement;
        movementStats = _movementStats;

        fov = _fov;
        fovStats = _fovStats;

        alarmedStats = _alarmedStats;
    }

    public override void EnterState()
    {
        isSmallPatrolling = false;
        patrollCounter = 0;

        movement.SetUpParameters(movementStats);
        movement.ClearPath();
        fov.SetUpStats(fovStats);
        fov.StartFindingTargets(fovStats.FindingDelay);
        movement.OnDestinationReached += OnReachDestination;
        movement.OnStopLookingAround += OnStopLookingAround;

        movement.SetEnemyDestination(alarmedStats.AlarmedPosition);
    }

    public override void ExitState()
    {
        alarmedStats.IsAlarmed = false;
        alarmedStats.AlarmedPosition = Vector3.zero;

        isSmallPatrolling = false;
        patrollCounter = 0;

        movement.OnDestinationReached -= OnReachDestination;
        movement.OnStopLookingAround -= OnStopLookingAround;
        fov.StopFindingTargets();
        movement.StopLookingAround();
        movement.StopCheckingDistance();
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        movement.UpdateMoveAnimation(movement.Agent.velocity.magnitude);
    }

    public override E_EnemyState GetNextState()
    {
        if (fov.GetVisibleTargets().Count > 0)
            return E_EnemyState.Chase;

        if (alarmedStats.IsAlarmed)
            return E_EnemyState.Alarmed;

        return E_EnemyState.Patrol;
    }


    private void OnReachDestination()
    {
        if (!isSmallPatrolling)
        {
            movement.ClearPath();
            for (int i = 0; i < alarmedStats.SmallPatrollCounter; i++)
            {
                movement.AddWaypoint(movement.FindRandomPointAroundPoint(Vector3.zero,2f));
            }
            movement.StartLookingAround(movementStats.LookingAroundTime);
            patrollCounter = 1;
            isSmallPatrolling = true;
        }
        else
        {
            patrollCounter += 1;
            movement.StartLookingAround(movementStats.LookingAroundTime);
        }
    }

    private void OnStopLookingAround()
    {
        if (patrollCounter == alarmedStats.SmallPatrollCounter - 1)
        {
            alarmedStats.IsAlarmed = false;
        }
        else
        {
            movement.GoToNextPoint();
        }
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
[Serializable]
public class EnemyAlarmedStats
{
    public bool IsAlarmed = false;
    public Vector3 AlarmedPosition = Vector3.zero;
    public int SmallPatrollCounter = 0;
}
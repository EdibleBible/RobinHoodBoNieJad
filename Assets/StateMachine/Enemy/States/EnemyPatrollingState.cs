using UnityEngine;

public class EnemyPatrollingState : BaseState<E_EnemyState>
{
    private EnemyMovement movement;
    private EnemyMovementStats movementStats;

    private FieldOfView fov;
    private EnemyFovStats fovStats;

    private EnemyAlarmedStats alarmedStats;

    public EnemyPatrollingState(EnemyMovement _movement, EnemyMovementStats _movementStats, FieldOfView _fov,
        EnemyFovStats _fovStats, EnemyAlarmedStats _alarmedStats) : base(E_EnemyState.Patrol)
    {
        movement = _movement;
        movementStats = _movementStats;
        fov = _fov;
        fovStats = _fovStats;
        alarmedStats = _alarmedStats;
    }

    public override void EnterState()
    {
        movement.SetUpParameters(movementStats);
        movement.PathFind();
        fov.SetUpStats(fovStats);
        fov.StartFindingTargets(fovStats.FindingDelay);
        movement.OnDestinationReached += OnReachDestination;
        movement.OnStopLookingAround += OnStopLookingAround;

        movement.GoToNextPoint();
    }

    public override void ExitState()
    {
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
        if (alarmedStats.IsAlarmed)
            return E_EnemyState.Alarmed;

        if (fov.GetVisibleTargets().Count > 0)
        {
            return E_EnemyState.Chase;
        }

        return E_EnemyState.Patrol;
    }

    private void OnReachDestination()
    {
        movement.StartLookingAround(movementStats.LookingAroundTime);
    }

    private void OnStopLookingAround()
    {
        movement.GoToNextPoint();
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
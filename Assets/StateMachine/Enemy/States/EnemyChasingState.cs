using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyChasingState : BaseState<E_EnemyState>
{
    private EnemyMovement movement;
    private EnemyMovementStats movementStats;

    private FieldOfView fov;
    private EnemyFovStats fovStats;

    private EnemyAlarmedStats alarmedStats;

    public EnemyChasingState(EnemyMovement _movement, EnemyMovementStats _movementStats, FieldOfView _fov,
        EnemyFovStats _fovStats, EnemyAlarmedStats _alarmedStats) : base(E_EnemyState.Chase)
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
        fov.SetUpStats(fovStats);
        fov.StartFindingTargets(fovStats.FindingDelay);
    }

    public override void ExitState()
    {
        movement.OnDestinationReached -= OnReachDestination;
        movement.OnStopLookingAround -= OnStopLookingAround;
        fov.StopFindingTargets();
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        movement.SetEnemyDestination(fov.GetVisibleTargets()[0].transform.position, false);
        movement.UpdateMoveAnimation(movement.Agent.velocity.magnitude);
    }

    public override E_EnemyState GetNextState()
    {
        if (fov.GetVisibleTargets().Count <= 0)
        {
            return E_EnemyState.Patrol;
        }

        return E_EnemyState.Chase;
    }

    private void OnReachDestination()
    {
        Debug.Log("Reach destination");
        movement.StartLookingAround(movementStats.LookingAroundTime);
    }

    private void OnStopLookingAround()
    {
        Debug.Log("Stop looking around");
        movement.GoToNextPoint();
    }

    public override void OnTriggerEnterState(Collider other)
    {
        if (other.TryGetComponent(out GameoverController gameOver))
        {
            gameOver.LoseGame();
        }
    }

    public override void OnTriggerStayState(Collider other)
    {
    }

    public override void OnTriggerExitState(Collider other)
    {
    }
}
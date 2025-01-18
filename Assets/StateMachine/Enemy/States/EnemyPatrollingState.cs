using UnityEngine;

public class EnemyPatrollingState : BaseState<E_EnemyState>
{
    private FieldOfView fov;
    private EnemyFovStats fovStats;
    private EnemyMovement enemyMovement;
    private EnemyMovementStats movementStats;
    private float findingPointDistance;


    public EnemyPatrollingState(FieldOfView _fov,EnemyFovStats _fovStats,EnemyMovement _enemyMovement, EnemyMovementStats _movementStats,
        float _findingPointDistance) : base(E_EnemyState.Patrol)
    {
        fov = _fov;
        fovStats = _fovStats;
        
        enemyMovement = _enemyMovement;
        movementStats = _movementStats;
        findingPointDistance = _findingPointDistance;
    }

    public override void EnterState()
    {
        fov.SetUpStats(fovStats);
        fov.ResetFindingTargets(fovStats.FindingDelay);
        
        enemyMovement.SetUpSpeed(movementStats.MaxSpeed, movementStats.Acceleration, movementStats.Deceleration,
            movementStats.AngularSpeed, movementStats.StoppingDistance);
        enemyMovement.ResetCheckingDistance(movementStats.DelayBettwenDistanceCheck);

        Vector3 randomPoint;
        do
            randomPoint = enemyMovement.FindRandomPointOnNavMesh(findingPointDistance);
        while (!enemyMovement.IsPointOnNavMesh(randomPoint));

        enemyMovement.SetDestination(randomPoint);
    }

    public override void ExitState()
    {
        enemyMovement.StopCheckingDistance();
        fov.StopFindingTargets();
    }

    public override void UpdateState()
    {
        if (enemyMovement.GetDistanceToDestination() <= movementStats.ChangeDestinationPointDistance)
        {
            Vector3 randomPoint = enemyMovement.FindRandomPointOnNavMesh(findingPointDistance);
            enemyMovement.ResetDestination();
            
            if (!enemyMovement.IsPointOnNavMesh(randomPoint))
                randomPoint = enemyMovement.FindRandomPointOnNavMesh(findingPointDistance);
            
            enemyMovement.SetDestination(randomPoint);
        }
    }

    public override E_EnemyState GetNextState()
    {
        if (fov.GetVisibleTargets().Count > 0)
            return E_EnemyState.Chase;
        else
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
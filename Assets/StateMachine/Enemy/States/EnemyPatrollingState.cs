using UnityEngine;

public class EnemyPatrollingState : BaseState<E_EnemyState>
{
    private FieldOfView fov;
    private EnemyFovStats fovStats;
    private EnemyMovement enemyMovement;
    private EnemyMovementStats movementStats;
    private EnemyPatrollingStats patrollingStats;
    private float findingPointDistance;


    public EnemyPatrollingState(FieldOfView _fov,EnemyFovStats _fovStats,EnemyMovement _enemyMovement, EnemyMovementStats _movementStats,EnemyPatrollingStats _patrollingStats,
        float _findingPointDistance) : base(E_EnemyState.Patrol)
    {
        fov = _fov;
        fovStats = _fovStats;
        
        enemyMovement = _enemyMovement;
        movementStats = _movementStats;
        findingPointDistance = _findingPointDistance;
        patrollingStats = _patrollingStats;
        patrollingStats.CreatePatrolPoint(_fov.transform.position);
    }

    public override void EnterState()
    {
        fov.SetUpStats(fovStats);
        fov.ResetFindingTargets(fovStats.FindingDelay);
        
        enemyMovement.SetUpSpeed(movementStats.MaxSpeed, movementStats.Acceleration,
            movementStats.AngularSpeed, movementStats.StoppingDistance);
        enemyMovement.ResetCheckingDistance(movementStats.DelayBettwenDistanceCheck);

        if (patrollingStats.RandomPointPatroll)
        {
            Vector3 randomPoint;
            do
                randomPoint = enemyMovement.FindRandomPointOnNavMesh(findingPointDistance);
            while (!enemyMovement.IsPointOnNavMesh(randomPoint));

            enemyMovement.SetDestination(randomPoint);
        }
        else
        {
            enemyMovement.SetDestination(patrollingStats.GetNearestPoint(fov.transform.position));
        }
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
            if (patrollingStats.RandomPointPatroll)
            {
                Vector3 randomPoint = enemyMovement.FindRandomPointOnNavMesh(findingPointDistance);
                enemyMovement.ResetDestination();
            
                if (!enemyMovement.IsPointOnNavMesh(randomPoint))
                    randomPoint = enemyMovement.FindRandomPointOnNavMesh(findingPointDistance);
            
                enemyMovement.SetDestination(randomPoint);
            }
            else
            {
                enemyMovement.SetDestination(patrollingStats.GetNextPatrollingPoint());
                enemyMovement.ResetDestination();
            }
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
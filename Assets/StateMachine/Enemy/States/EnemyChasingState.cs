using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyChasingState : BaseState<E_EnemyState>
{
    private EnemyMovement enemyMovement;
    private EnemyMovementStats movementStats;
    private FieldOfView fov;
    private EnemyFovStats fovStats;
    
    public EnemyChasingState(FieldOfView _fov,EnemyFovStats _enemyFovStats ,EnemyMovement _enemyMovement, EnemyMovementStats _movementStats) : base(E_EnemyState.Chase)
    {
        enemyMovement = _enemyMovement;
        movementStats = _movementStats;
        fov = _fov;
        fovStats = _enemyFovStats;
    }
    
    public override void EnterState()
    {
        fov.SetUpStats(fovStats);
        enemyMovement.SetUpSpeed(movementStats.MaxSpeed, movementStats.Acceleration, movementStats.Deceleration,
            movementStats.AngularSpeed, movementStats.StoppingDistance);
        
        
        enemyMovement.ResetCheckingDistance(movementStats.DelayBettwenDistanceCheck);
        fov.StartFindingTargets(fovStats.FindingDelay);
        
        enemyMovement.SetDestination(fov.GetVisibleTargets()[0].transform.position);
        
        
        Debug.Log("Chase State Enter");
    }

    public override void ExitState()
    {
        fov.StopFindingTargets();
        enemyMovement.StopCheckingDistance();
    }

    public override void UpdateState()
    {
        enemyMovement.SetDestination(fov.GetVisibleTargets()[0].transform.position);
    }

    public override E_EnemyState GetNextState()
    {
        if (fov.GetVisibleTargets().Count <= 0)
        {
            return E_EnemyState.Patrol;
        }
        else
        {
            return stateKey;   
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

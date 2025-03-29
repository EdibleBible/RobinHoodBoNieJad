using UnityEngine;

public class CageEnemyLookAroundState : BaseState<E_CageEnemyState>
{
    private float lookAroundAnimationSpeed;
    private EnemyFovStats fovStats;
    private CageEnemyAnimationController animationController;
    private FieldOfView fov;

    public CageEnemyLookAroundState(CageEnemyAnimationController _animationController, FieldOfView _fov,
        EnemyFovStats _fovStats, float _lookAroundAnimationSpeed) : base(E_CageEnemyState.LookAround)
    {
        animationController = _animationController;
        fov = _fov;
        fovStats = _fovStats;
        lookAroundAnimationSpeed = _lookAroundAnimationSpeed;
        
        
    }

    public override void EnterState()
    {
        fov.SetUpStats(fovStats);
        fov.StartFindingTargets(fovStats.FindingDelay);
        animationController.SetTrigger("Looking");
        animationController.SetSpeed("AnimationSpeed", lookAroundAnimationSpeed);
        
        //TEMPOLARY FINDING ENEMY
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }

    public override E_CageEnemyState GetNextState()
    {
        if (fov.GetVisibleTargets().Count > 0)
            return E_CageEnemyState.Trigger;
        
        return E_CageEnemyState.LookAround;
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
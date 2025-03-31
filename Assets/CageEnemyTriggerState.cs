using UnityEngine;

public class CageEnemyTriggerState : BaseState<E_CageEnemyState>
{
    private EnemyFovStats fovStats;
    private CageEnemyAnimationController animationController;
    private FieldOfView fov;


    public CageEnemyTriggerState(CageEnemyAnimationController _animationController, FieldOfView _fov,
        EnemyFovStats _fovStats) : base(E_CageEnemyState.Trigger)
    {
        animationController = _animationController;
        fov = _fov;
        fovStats = _fovStats;
    }

    public override void EnterState()
    {
        fov.SetUpStats(fovStats);
        animationController.SetTrigger("Enrage");
        
        var enemies = GameObject.FindObjectsByType<EnemyStateMachineController>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            enemy.alarmedStateStats.IsAlarmed = true;
            enemy.alarmedStateStats.AlarmedPosition = fov.GetTargePosition(0);        
        }
    }

    public override void ExitState()
    {
        animationController.SetEnrageFalse();
    }

    public override void UpdateState()
    {
    }

    public override E_CageEnemyState GetNextState()
    {
        if (!animationController.IsEnrage && fov.GetVisibleTargets().Count == 0)
            return E_CageEnemyState.LookAround;

        return E_CageEnemyState.Trigger;
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
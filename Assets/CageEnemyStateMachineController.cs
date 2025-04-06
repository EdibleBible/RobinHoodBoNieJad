using System;
using UnityEngine;

public class CageEnemyStateMachineController : StateManager<E_CageEnemyState>
{
    private CageEnemyLookAroundState _lookAroundState;
    private CageEnemyTriggerState _triggerState;

    private FieldOfView fov;
    private CageEnemyAnimationController animationController;

    [Header("look around state settings")]
    [SerializeField]
    private float lookAroundAnimationSpeed;
    [SerializeField]
    private EnemyFovStats fovLookAroundStat;

    [Header("trigger state settings")]
    [SerializeField]
    private EnemyFovStats fovTriggerStat;

    private void Awake()
    {
        fov = GetComponent<FieldOfView>();
        animationController = GetComponent<CageEnemyAnimationController>();
    }

    public override void Start()
    {
        _lookAroundState =
            new CageEnemyLookAroundState(animationController, fov, fovLookAroundStat, lookAroundAnimationSpeed);
        _triggerState = new CageEnemyTriggerState(animationController, fov, fovTriggerStat);

        state.Add(E_CageEnemyState.LookAround, _lookAroundState);
        state.Add(E_CageEnemyState.Trigger, _triggerState);

        currentState = _lookAroundState;
        base.Start();
    }

    public override void Update()
    {
        E_CageEnemyState nextStateKey = currentState.GetNextState();
        if (!isTransitioningState && nextStateKey.Equals(currentState.stateKey))
        {
            currentState.UpdateState();
        }
        else if (!isTransitioningState)
        {
            TransitionToState(nextStateKey);
        }
    }
}
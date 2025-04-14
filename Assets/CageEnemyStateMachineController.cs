using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class CageEnemyStateMachineController : StateManager<E_CageEnemyState>
{
    private CageEnemyLookAroundState _lookAroundState;
    private CageEnemyTriggerState _triggerState;

    private FieldOfView fov;
    private CageEnemyAnimationController animationController;

    [Header("look around state settings")]
    [SerializeField] private float lookAroundAnimationSpeed;
    [SerializeField] private EnemyFovStats fovLookAroundStat;

    [Header("trigger state settings")]
    [SerializeField] private EnemyFovStats fovTriggerStat;

    [Header("FMOD settings")]
    [SerializeField] private string alertLoopEventPath = "event:/Enemy/AlertLoop";

    private EventInstance alertLoopInstance;
    private bool isAlertLoopPlaying = false;

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
            HandleSoundOnStateChange(currentState.stateKey, nextStateKey);
            TransitionToState(nextStateKey);
        }
    }

    private void HandleSoundOnStateChange(E_CageEnemyState from, E_CageEnemyState to)
    {
        if (from != E_CageEnemyState.Trigger && to == E_CageEnemyState.Trigger)
        {
            // Start loop if entering Trigger state
            if (!isAlertLoopPlaying)
            {
                alertLoopInstance = RuntimeManager.CreateInstance(alertLoopEventPath);
                RuntimeManager.AttachInstanceToGameObject(alertLoopInstance, transform, GetComponent<Rigidbody>());
                alertLoopInstance.start();
                isAlertLoopPlaying = true;
            }
        }
        else if (from == E_CageEnemyState.Trigger && to != E_CageEnemyState.Trigger)
        {
            // Stop loop if leaving Trigger state
            if (isAlertLoopPlaying)
            {
                alertLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                alertLoopInstance.release();
                isAlertLoopPlaying = false;
            }
        }
    }
}

using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerFootstepsSound : MonoBehaviour
{
    [SerializeField] private PlayerStateMachineController playerStateMachine;

    [SerializeField] private EventReference walkLoop;
    [SerializeField] private EventReference runLoop;
    [SerializeField] private EventReference crouchLoop;

    private EventInstance currentStepInstance;
    private E_PlayerState lastState;
    private bool isPlaying;

    private void Update()
    {
        bool isMovingForward = Input.GetKey(KeyCode.W);

        if (isMovingForward)
        {
            var currentState = playerStateMachine.currentState.stateKey;

            // Only switch loop if the state has changed
            if (currentState != lastState || !isPlaying)
            {
                StartStepLoop(currentState);
            }
        }
        else
        {
            StopStepLoop();
        }
    }

    private void StartStepLoop(E_PlayerState state)
    {
        StopStepLoop(); // stop previous if any

        switch (state)
        {
            case E_PlayerState.Walk:
                currentStepInstance = RuntimeManager.CreateInstance(walkLoop);
                break;
            case E_PlayerState.Running:
                currentStepInstance = RuntimeManager.CreateInstance(runLoop);
                break;
            case E_PlayerState.Crouching:
                currentStepInstance = RuntimeManager.CreateInstance(crouchLoop);
                break;
            default:
                return;
        }

        currentStepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        currentStepInstance.start();
        isPlaying = true;
        lastState = state;
    }

    private void StopStepLoop()
    {
        if (isPlaying)
        {
            currentStepInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentStepInstance.release();
            isPlaying = false;
        }
    }

    private void OnDisable()
    {
        StopStepLoop();
    }
}

using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachineController : StateManager<E_PlayerState>
{
    PlayerRunningState _playerRunningState;
    PlayerWalkState _playerWalkState;
    PlayerCrouchState _playerCrouchState;
    PlayerDoorInteraction _playerDoorInteraction;

    [Header("Walk State")] [SerializeField]
    float walkSpeed;

    [SerializeField] float walkAcceleration;
    [SerializeField] float walkDeceleration;

    [Header("Crouch State")] 
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchAcceleration;
    [SerializeField] float crouchDeceleration;
    [SerializeField] Transform lookAtTarget;
    [SerializeField] Transform followTarget;
    [SerializeField] Vector3 cameraOffset;

    [Header("Run State")] [SerializeField] float runSpeed;
    [SerializeField] float runAcceleration;
    [SerializeField] float runDeceleration;

    private PlayerWalk playerWalk;
    private PlayerRotation playerRotation;
    private PlayerAnimatorController playerAnimatorController;

    public override void Start()
    {
        playerWalk = gameObject.GetComponent<PlayerWalk>();
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        playerRotation = GetComponent<PlayerRotation>();

        _playerWalkState = new PlayerWalkState(walkSpeed, walkAcceleration, walkDeceleration, playerWalk,
            playerAnimatorController, playerRotation);

        _playerRunningState = new PlayerRunningState(runSpeed, runAcceleration, runDeceleration, playerWalk,
            playerAnimatorController, playerRotation);

        _playerCrouchState = new PlayerCrouchState(crouchSpeed, crouchAcceleration, crouchDeceleration, playerWalk,
            playerAnimatorController, playerRotation, lookAtTarget,followTarget, cameraOffset);

        _playerDoorInteraction = new PlayerDoorInteraction(playerWalk, playerAnimatorController);

        state.Add(E_PlayerState.Running, _playerRunningState);
        state.Add(E_PlayerState.Crouching, _playerCrouchState);
        state.Add(E_PlayerState.Walk, _playerWalkState);
        state.Add(E_PlayerState.OpenDoorInteraction, _playerDoorInteraction);


        currentState = _playerWalkState;
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }
}
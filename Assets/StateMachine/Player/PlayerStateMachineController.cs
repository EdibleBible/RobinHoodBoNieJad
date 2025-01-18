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

    [Header("Crouch State")] [SerializeField]
    float crouchSpeed;
    [SerializeField] float crouchAcceleration;
    [SerializeField] float crouchDeceleration;

    [Header("Run State")] [SerializeField] float runSpeed;
    [SerializeField] float runAcceleration;
    [SerializeField] float runDeceleration;

    private PlayerWalk playerWalk;
    private PlayerAnimatorController playerAnimatorController;

    public override void Start()
    {
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        playerWalk = GetComponent<PlayerWalk>();

        _playerWalkState = new PlayerWalkState(walkSpeed, walkAcceleration, walkDeceleration, playerWalk,
            playerAnimatorController);
        
        _playerRunningState = new PlayerRunningState(runSpeed, runAcceleration, runDeceleration, playerWalk,
            playerAnimatorController);
        
        _playerCrouchState = new PlayerCrouchState(crouchSpeed, crouchAcceleration, crouchDeceleration, playerWalk,
            playerAnimatorController);
        
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
        Debug.Log(currentState.stateKey);
        base.Update();
    }
}
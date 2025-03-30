using UnityEngine;

public class PlayerRunningState : BaseState<E_PlayerState>
{
    public float MovementSpeed { get; set; }
    public float AccelerationTime { get; set; }
    public float DecelerationTime { get; set; }
    public float UsedStaminaInRun { get; set; }
    public PlayerWalk PlayerWalk { get; set; }
    public PlayerRotation PlayerRotation { get; set; }
    public PlayerAnimatorController PlayerAnimatorController { get; set; }
    public PlayerStaminaSystem PlayerStaminaSystem { get; set; }

    public PlayerRunningState(float movementSpeed,float usedStaminaInRun, float accelerationTime, float decelerationTime,
        PlayerWalk playerWalk, PlayerAnimatorController playerAnimatorController, PlayerRotation playerRotation,
        PlayerStaminaSystem playerStamina) : base(E_PlayerState.Running)
    {
        MovementSpeed = movementSpeed;
        AccelerationTime = accelerationTime;
        DecelerationTime = decelerationTime;
        PlayerWalk = playerWalk;
        PlayerAnimatorController = playerAnimatorController;
        PlayerRotation = playerRotation;
        PlayerStaminaSystem = playerStamina;
        UsedStaminaInRun = usedStaminaInRun;
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        PlayerWalk.Movement(MovementSpeed, AccelerationTime, DecelerationTime, out float x, out float y);
        var velocity = PlayerWalk.GetCharacterVelocity();
        Transform transform = PlayerWalk.GetTransform();

        float velocityX = transform.InverseTransformDirection(velocity).x;
        float velocityZ = transform.InverseTransformDirection(velocity).z;

        if (velocity.magnitude > 0)
        {
            PlayerStaminaSystem.UseStamina(UsedStaminaInRun);
        }

        PlayerRotation.UpdateRotation();

        PlayerAnimatorController.UpdateWalkParameters(x, y);
        PlayerAnimatorController.UpdateCrouchParameters(velocityX, velocityZ, false);
    }

    public override E_PlayerState GetNextState()
    {
        if (PlayerStaminaSystem.currentStamina <= 0.1)
        {
            return E_PlayerState.Walk;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && PlayerStaminaSystem.currentStamina > 0.1)
        {
            return E_PlayerState.Running;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            return E_PlayerState.Crouching;
        }
        else
        {
            return E_PlayerState.Walk;
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
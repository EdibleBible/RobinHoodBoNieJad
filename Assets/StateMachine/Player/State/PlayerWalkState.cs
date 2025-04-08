using UnityEngine;

public class PlayerWalkState : BaseState<E_PlayerState>
{
    public float MovementSpeed { get; set; }
    public float AccelerationTime { get; set; }
    public float DecelerationTime { get; set; }
    public PlayerWalk PlayerWalk { get; set; }

    public PlayerRotation PLayerRotation;
    public PlayerAnimatorController PlayerAnimatorController { get; set; }
    public PlayerStaminaSystem PlayerStamina { get; set; }

    public PlayerWalkState(float movementSpeed, float accelerationTime, float decelerationTime, PlayerWalk playerWalk,
        PlayerAnimatorController playerAnimatorController, PlayerRotation playerRotation,
        PlayerStaminaSystem playerStamina) : base(E_PlayerState.Walk)
    {
        MovementSpeed = movementSpeed;
        AccelerationTime = accelerationTime;
        DecelerationTime = decelerationTime;
        PlayerWalk = playerWalk;
        PLayerRotation = playerRotation;
        PlayerAnimatorController = playerAnimatorController;
        PlayerStamina = playerStamina;
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
        Vector3 velocity = PlayerWalk.GetCharacterVelocity();
        Transform transform = PlayerWalk.GetTransform();

        float velocityX = transform.InverseTransformDirection(velocity).x;
        float velocityZ = transform.InverseTransformDirection(velocity).z;

        PLayerRotation.UpdateRotation();

        PlayerAnimatorController.UpdateWalkParameters(x, y);
        PlayerAnimatorController.UpdateCrouchParameters(velocityX, velocityZ, false);
    }

    public override void FixedUpdateState()
    {
    }

    public override E_PlayerState GetNextState()
    {
        if (PlayerStamina != null)
        {
            if (Input.GetKey(KeyCode.LeftShift) && PlayerStamina.currentStamina > 0.1)
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
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
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
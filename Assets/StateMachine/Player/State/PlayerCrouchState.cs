using UnityEngine;

public class PlayerCrouchState : BaseState<E_PlayerState>
{
    public float MovementSpeed { get; set; }
    public float AccelerationTime { get; set; }
    public float DecelerationTime { get; set; }
    public PlayerWalk PlayerWalk { get; set; }

    public PlayerAnimatorController PlayerAnimatorController { get; set; }

    public PlayerCrouchState(float movementSpeed, float accelerationTime, float decelerationTime, PlayerWalk playerWalk,
        PlayerAnimatorController playerAnimatorController) : base(E_PlayerState.Crouching)
    {
        MovementSpeed = movementSpeed;
        AccelerationTime = accelerationTime;
        DecelerationTime = decelerationTime;
        PlayerWalk = playerWalk;
        PlayerAnimatorController = playerAnimatorController;
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
        
        PlayerAnimatorController.UpdateCrouchParameters(x,y, true);
    }

    public override E_PlayerState GetNextState()
    {
        if (Input.GetKey(KeyCode.LeftControl))
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
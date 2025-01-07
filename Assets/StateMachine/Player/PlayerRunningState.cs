using UnityEngine;

public class PlayerRunningState : BaseState<E_PlayerState>
{
    public float MovementSpeed { get; set; }
    public float AccelerationTime { get; set; }
    public float DecelerationTime { get; set; }
    public PlayerWalk PlayerWalk { get; set; }
    
    public PlayerAnimatorController PlayerAnimatorController { get; set; }
    
    public PlayerRunningState(float movementSpeed,float accelerationTime, float decelerationTime, PlayerWalk playerWalk, PlayerAnimatorController playerAnimatorController) : base(E_PlayerState.Running)
    {
        MovementSpeed = movementSpeed;
        AccelerationTime = accelerationTime;
        DecelerationTime = decelerationTime;
        PlayerWalk = playerWalk;
        PlayerAnimatorController = playerAnimatorController;
    }
    
    public override void EnterState()
    {
        Debug.Log("Entered PlayerRunningState");
    }

    public override void ExitState()
    {
        Debug.Log("Exited PlayerWalkState");
    }

    public override void UpdateState()
    {
        PlayerWalk.Movement(MovementSpeed, AccelerationTime, DecelerationTime);
        var velocity = PlayerWalk.GetCharacterVelocity(); 
        Transform transform = PlayerWalk.GetTransform();
        
        float velocityX = transform.InverseTransformDirection(velocity).x;
        float velocityZ = transform.InverseTransformDirection(velocity).z;
        
        PlayerAnimatorController.UpdateWalkParameters(velocityX, velocityZ);
        PlayerAnimatorController.UpdateCrouchParameters(velocityX, velocityZ, false);

    }

    public override E_PlayerState GetNextState()
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
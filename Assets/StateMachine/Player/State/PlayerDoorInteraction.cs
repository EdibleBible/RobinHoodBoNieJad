using UnityEngine;

public class PlayerDoorInteraction : BaseState<E_PlayerState>
{
    public PlayerAnimatorController PlayerAnimatorController;
    public PlayerWalk PlayerWalk;
    
    public PlayerDoorInteraction(PlayerWalk playerWalk,PlayerAnimatorController playerAnimatorController) : base(E_PlayerState.OpenDoorInteraction)
    {
        PlayerAnimatorController = playerAnimatorController;
        PlayerWalk = playerWalk;
    }
    public override void EnterState()
    {
        PlayerWalk.SetMotion(false);
        Debug.Log("Door Open State Entered");
    }

    public override void ExitState()
    {
        PlayerWalk.SetMotion(true);
        Debug.Log("Door Open State Exited");
    }

    public override void UpdateState()
    {
        PlayerWalk.Movement(0,0,0 ,out float xVel, out float yVel);
        
    }

    public override void FixedUpdateState()
    {
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

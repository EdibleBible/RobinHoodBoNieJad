using UnityEngine;
using DG.Tweening;

public class PlayerCrouchState : BaseState<E_PlayerState>
{
    public float MovementSpeed { get; set; }
    public float AccelerationTime { get; set; }
    public float DecelerationTime { get; set; }
    public PlayerWalk PlayerWalk { get; set; }
    public PlayerRotation PlayerRotation { get; set; }
    public PlayerAnimatorController PlayerAnimatorController { get; set; }

    public Transform LookAtTarget { get; set; }
    public Transform FollowTarget { get; set; }
    public Vector3 MoveCameraOffset { get; set; }

    private Tween lookAtTween;
    private Tween followTween;

    public PlayerCrouchState(float movementSpeed, float accelerationTime, float decelerationTime, PlayerWalk playerWalk,
        PlayerAnimatorController playerAnimatorController, PlayerRotation playerRotation, Transform lookAt,
        Transform follow, Vector3 moveCameraOffset) : base(E_PlayerState.Crouching)
    {
        MovementSpeed = movementSpeed;
        AccelerationTime = accelerationTime;
        DecelerationTime = decelerationTime;
        PlayerWalk = playerWalk;
        PlayerAnimatorController = playerAnimatorController;
        PlayerRotation = playerRotation;

        LookAtTarget = lookAt;
        FollowTarget = follow;
        MoveCameraOffset = moveCameraOffset;
    }

    public override void EnterState()
    {
        lookAtTween?.Kill();
        followTween?.Kill();
        
        PlayerRotation.SetCrouch(true);
    }

    public override void ExitState()
    {
        lookAtTween?.Kill();
        followTween?.Kill();
        
        PlayerRotation.SetCrouch(false);

    }

    public override void UpdateState()
    {
        PlayerWalk.Movement(MovementSpeed, AccelerationTime, DecelerationTime, out float x, out float y);
        Vector3 velocity = PlayerWalk.GetCharacterVelocity();
        Transform transform = PlayerWalk.GetTransform();

        float velocityX = transform.InverseTransformDirection(velocity).x;
        float velocityZ = transform.InverseTransformDirection(velocity).z;

        PlayerRotation.UpdateRotation();

        PlayerAnimatorController.UpdateCrouchParameters(velocityX, velocityZ, true);
    }

    public override void FixedUpdateState()
    {
    }

    public override E_PlayerState GetNextState()
    {
        if (PlayerWalk.Crouching)
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

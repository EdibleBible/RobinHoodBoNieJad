using System;
using UnityEngine;
using FMODUnity;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerStateMachineController playerStateMachine;
    
    [SerializeField] private Transform leftFootRaycast;
    [SerializeField] private Transform rightFootRaycast;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask groundLayerMask;
    
    [SerializeField] private EventReference walkStepSound;
    [SerializeField] private EventReference runStepSound;
    [SerializeField] private EventReference crouchStepSound;
    
    private bool leftFootOnGround;
    private bool rightFootOnGround;
    private bool leftFootLifted;
    private bool rightFootLifted;
    
    private float lastStepTime = 0f;
    [SerializeField] private float stepCooldown = 0.2f;
    
    private void Update()
    {
        CheckFootContact(leftFootRaycast, ref leftFootOnGround, ref leftFootLifted);
        CheckFootContact(rightFootRaycast, ref rightFootOnGround, ref rightFootLifted);
    }

    private void CheckFootContact(Transform foot, ref bool isFootOnGround, ref bool footLifted)
    {
        RaycastHit hit;
        bool hitGround = Physics.Raycast(foot.position, Vector3.down, out hit, rayDistance, groundLayerMask);

        if (hitGround)
        {
            if (!isFootOnGround && footLifted && Time.time >= lastStepTime + stepCooldown)
            {
                PlayFootstepSound(hit.point);
                lastStepTime = Time.time;
                footLifted = false;
            }
            isFootOnGround = true;
        }
        else
        {
            if (isFootOnGround)
            {
                footLifted = true;
            }
            isFootOnGround = false;
        }
    }

    private void PlayFootstepSound(Vector3 position)
    {
        switch (playerStateMachine.currentState.stateKey)
        {
            case E_PlayerState.Walk:
                RuntimeManager.PlayOneShot(walkStepSound, position);
                break;
            case E_PlayerState.Running:
                RuntimeManager.PlayOneShot(runStepSound, position);
                break;
            case E_PlayerState.Crouching:
                RuntimeManager.PlayOneShot(crouchStepSound, position);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (animator == null)
            return;

        Gizmos.color = leftFootOnGround ? Color.red : Color.white;
        Gizmos.DrawRay(leftFootRaycast.position, Vector3.down * rayDistance);
        Gizmos.color = rightFootOnGround ? Color.red : Color.white;
        Gizmos.DrawRay(rightFootRaycast.position, Vector3.down * rayDistance);
    }
}

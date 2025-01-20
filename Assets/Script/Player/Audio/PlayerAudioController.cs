using System;
using UnityEngine;
using FMODUnity;
using UnityEngine.Rendering;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private Transform leftFootRaycast;
    [SerializeField] private Transform rightFootRaycast;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask groundLayerMask;

    [SerializeField] private EventReference walkStepSound;
    [SerializeField] private EventReference runStepSound;
    [SerializeField] private EventReference crouchStepSound;

    private PlayerStateMachineController playerStateMachine;

    public bool DEBUG;

    private void Awake()
    {
        playerStateMachine = GetComponent<PlayerStateMachineController>();
    }

    private bool leftFootOnGround = false;
    private bool rightFootOnGround = false;
    void Update()
    {
        CheckFootContact(leftFootRaycast, ref leftFootOnGround);
        CheckFootContact(rightFootRaycast, ref rightFootOnGround);
    }
    private void CheckFootContact(Transform foot, ref bool isFootOnGround)
    {
        RaycastHit hit;
        bool hitGround = Physics.Raycast(foot.position, Vector3.down, out hit, rayDistance, groundLayerMask);

        if (hitGround && !isFootOnGround)
        {
            // Stopa dotknęła ziemi, odtwarzamy dźwięk
            PlayFootstepSound(hit.point);
            isFootOnGround = true;
        }
        else if (!hitGround)
        {
            // Stopa jest w powietrzu
            isFootOnGround = false;
        }
    }
    private void PlayFootstepSound(Vector3 position)
    {
        if (playerStateMachine.currentState.stateKey == E_PlayerState.Walk)
            RuntimeManager.PlayOneShot(walkStepSound, position);
        else if (playerStateMachine.currentState.stateKey == E_PlayerState.Running)
            RuntimeManager.PlayOneShot(runStepSound, position);
        else if (playerStateMachine.currentState.stateKey == E_PlayerState.Crouching)
            RuntimeManager.PlayOneShot(crouchStepSound, position);
    }

    private void OnDrawGizmos()
    {
        if (DEBUG)
        {
            Gizmos.DrawRay(leftFootRaycast.position, Vector3.down * rayDistance);
            Gizmos.DrawRay(rightFootRaycast.position, Vector3.down * rayDistance);
        }
    }
}
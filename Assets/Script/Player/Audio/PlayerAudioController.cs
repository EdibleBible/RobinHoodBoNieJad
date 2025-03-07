using System;
using System.Collections;
using System.Collections.Generic;
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

    private bool canCheck;

    public bool DEBUG;

    private void Awake()
    {
        playerStateMachine = GetComponent<PlayerStateMachineController>();
    }

    /*private void Start()
    {
        StartCoroutine(CanCheck());
    }

    private IEnumerator CanCheck()
    {
        yield return new WaitForSeconds(1f);
        canCheck = true;
    }*/

    private bool leftFootOnGround = true;
    private bool rightFootOnGround = true;
    void Update()
    {
        /*if(!canCheck)
            return;*/
        
        CheckFootContact(leftFootRaycast, ref leftFootOnGround);
        CheckFootContact(rightFootRaycast, ref rightFootOnGround);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            PlayFootstepSound(transform.position);
        }
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
        if (DEBUG)
        {
            Gizmos.DrawRay(leftFootRaycast.position, Vector3.down * rayDistance);
            Gizmos.DrawRay(rightFootRaycast.position, Vector3.down * rayDistance);
        }
    }
}
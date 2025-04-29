using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    /*[Header("IK Position")]
    [SerializeField] private Transform leftHandIKPosition;
    [SerializeField] private Transform rightHandIKPosition;
    
    [Header("IK Weight")]
    [SerializeField] private float leftHandIKWeight;
    [SerializeField] private float rightHandIKWeight;*/
    
    private Animator animator;
    
    [Header("Feet IK")]
    [SerializeField] private LayerMask groundIKLayer;
    [SerializeField] private float raycastDistance = 1.5f;
    [SerializeField] private bool DEBUG_FEET;
    
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }


    public void UpdateWalkParameters(float velocityX, float velocityZ)
    {
        animator.SetFloat("Velocity X", velocityX);
        animator.SetFloat("Velocity Z", velocityZ);
    }

    public void UpdateCrouchParameters(float velocityX, float velocityZ, bool isCrouching)
    {
        animator.SetBool("IsCrouch", isCrouching);
        
        animator.SetFloat("Velocity X", velocityX);
        animator.SetFloat("Velocity Z", velocityZ);
    }

    public void ToogleTorch(bool isOn)
    {
        if (isOn)
        {
            animator.SetTrigger("EquipTorch");
        }
        else
        {
            animator.SetTrigger("HideTorch");
        }
    }
    // Feet IK
    
    //DoorOpen and IK

    /*public void OpenDoorInteraction()
    {
        animator.SetTrigger("OpenDoor");
    }

    public void SetDoorInteractionTrue()
    {
        IsOpenDoorInteraction = true;
    }

    public void SetDoorInteractionFalse()
    {
        IsOpenDoorInteraction = false;
    }*/
}
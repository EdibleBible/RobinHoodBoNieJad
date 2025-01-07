using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void UpdateAnimatorParameters(float velocityX, float velocityZ)
    {
        animator.SetFloat("Velocity X", velocityZ);
        animator.SetFloat("Velocity Z", velocityX);
    }
}

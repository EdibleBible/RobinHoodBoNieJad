using System;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateWalkParameters(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void SetLookAround(bool isLookingAround)
    {
        animator.SetBool("IsLookingAround", isLookingAround);
    }
}
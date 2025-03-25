using System;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            SetLookAround(true);
        else if (Input.GetKeyUp(KeyCode.Z))
            SetLookAround(false);
    }*/

    public void UpdateWalkParameters(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void SetLookAround(bool isLookingAround)
    {
        animator.SetBool("IsLookingAround", isLookingAround);
    }
}
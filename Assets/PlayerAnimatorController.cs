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


    public void UpdateWalkParameters(float velocityX, float velocityZ, float smoothTime = 0.1f)
    {
        // Płynne przejście wartości za pomocą Lerp
        float currentVelocityX = animator.GetFloat("Velocity X");
        float currentVelocityZ = animator.GetFloat("Velocity Z");

        // Używamy Mathf.Lerp do interpolacji wartości
        float newVelocityX = Mathf.Lerp(currentVelocityX, velocityZ, smoothTime);
        float newVelocityZ = Mathf.Lerp(currentVelocityZ, velocityX, smoothTime);

        // Ustawiamy wartości w animatorze
        animator.SetFloat("Velocity X", newVelocityX);
        animator.SetFloat("Velocity Z", newVelocityZ);
    }

    public void UpdateCrouchParameters(float velocityX, float velocityZ, bool isCrouching, float smoothTime = 0.1f)
    {
        // Płynne przejście wartości za pomocą Lerp
        float currentVelocityX = animator.GetFloat("Velocity X");
        float currentVelocityZ = animator.GetFloat("Velocity Z");

        // Ustawiamy animację kucania z wartością bool
        animator.SetBool("IsCrouch", isCrouching);

        // Używamy Mathf.Lerp do interpolacji prędkości
        float newVelocityX = Mathf.Lerp(currentVelocityX, velocityZ, smoothTime);
        float newVelocityZ = Mathf.Lerp(currentVelocityZ, velocityX, smoothTime);

        // Ustawiamy nowe wartości w animatorze
        animator.SetFloat("Velocity X", newVelocityX);
        animator.SetFloat("Velocity Z", newVelocityZ);
    }
}
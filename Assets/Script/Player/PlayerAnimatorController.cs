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


    public void UpdateWalkParameters(float velocityX, float velocityZ)
    {
        animator.SetFloat("Velocity X", velocityX);
        animator.SetFloat("Velocity Z", velocityZ);
        
        /*// Płynne przejście wartości za pomocą Lerp
        float currentVelocityX = animator.GetFloat("Velocity X");
        float currentVelocityZ = animator.GetFloat("Velocity Z");

        // Używamy Mathf.Lerp do interpolacji wartości
        float newVelocityX = Mathf.Lerp(currentVelocityX, velocityX, smoothTime * Time.deltaTime);
        Debug.LogWarning("X velocity: " + newVelocityX);
        float newVelocityZ = Mathf.Lerp(currentVelocityZ, velocityZ, smoothTime * Time.deltaTime);
        Debug.LogWarning("Y velocity: " + velocityZ);


        // Ustawiamy wartości w animatorze
        animator.SetFloat("Velocity X", newVelocityX);
        animator.SetFloat("Velocity Z", newVelocityZ);*/
    }

    public void UpdateCrouchParameters(float velocityX, float velocityZ, bool isCrouching)
    {
        animator.SetBool("IsCrouch", isCrouching);
        
        animator.SetFloat("Velocity X", velocityX);
        animator.SetFloat("Velocity Z", velocityZ);
        /*// Płynne przejście wartości za pomocą Lerp
        float currentVelocityX = animator.GetFloat("Velocity X");
        float currentVelocityZ = animator.GetFloat("Velocity Z");

        // Ustawiamy animację kucania z wartością bool
        animator.SetBool("IsCrouch", isCrouching);

        // Używamy Mathf.Lerp do interpolacji prędkości
        float newVelocityX = Mathf.Lerp(currentVelocityX, velocityX, smoothTime * Time.deltaTime);
        float newVelocityZ = Mathf.Lerp(currentVelocityZ, velocityZ, smoothTime * Time.deltaTime);

        // Ustawiamy nowe wartości w animatorze
        animator.SetFloat("Velocity X", velocityX);
        animator.SetFloat("Velocity Z", velocityZ);*/
    }
}
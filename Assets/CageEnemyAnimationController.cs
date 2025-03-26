using System.Collections;
using UnityEngine;

public class CageEnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    public bool IsEnrage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSpeed(string speedParameterName, float animationSpeed)
    {
        animator.SetFloat(speedParameterName, animationSpeed);
    }

    public void SetTrigger(string triggerParameterName, float expirationTime = 0.2f)
    {
        animator.SetTrigger(triggerParameterName);
        StartCoroutine(ResetTriggerAfterTime(triggerParameterName, expirationTime));
    }

    private IEnumerator ResetTriggerAfterTime(string triggerParameterName, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.ResetTrigger(triggerParameterName);
    }


    public void SetEnrageTrue()
    {
        IsEnrage = true;
    }
    
    public void SetEnrageFalse()
    {
        IsEnrage = false;
    }
}
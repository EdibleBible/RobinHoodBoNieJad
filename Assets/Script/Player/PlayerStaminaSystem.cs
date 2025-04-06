using System;
using UnityEngine;
using System.Collections;
using Script.ScriptableObjects;
using UnityEngine.Serialization;

public class PlayerStaminaSystem : MonoBehaviour
{
    [SerializeField]
    private float baseMaxStamina;
    private float maxStamina;
    [SerializeField]
    private float staminaRegenRate = 10f;
    [SerializeField]
    private float regenDelay = 2f;
    [SerializeField]
    private SOPlayerStatsController playerStatsController;

    [SerializeField]
    private GameEvent changeStaminaEvent;
    [SerializeField]
    private GameEvent changeMaxStaminaEvent;

    public float currentStamina;
    private bool isRegenerating;
    private Coroutine regenCoroutine;

    private void Start()
    {
        SetUpStamina(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseStamina(20);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var newMaxStamina = maxStamina + 20;
            ResetMaxStamina(newMaxStamina);
        }
    }

    public void SetUpStamina(bool resetCurrStamina = false)
    {
        var oldmaxStamina = maxStamina;
        maxStamina = (baseMaxStamina +
                      (int)Math.Floor(playerStatsController.GetSOPlayerStats(E_ModifiersType.Stamina).Additive)) *
                     (int)Math.Floor(playerStatsController.GetSOPlayerStats(E_ModifiersType.Stamina).Multiplicative);

        if (resetCurrStamina)
            currentStamina = maxStamina;

        changeMaxStaminaEvent.Raise(this, (maxStamina, oldmaxStamina));
        changeStaminaEvent.Raise(this, (maxStamina, currentStamina));
    }

    public void UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }

            changeStaminaEvent.Raise(this, (maxStamina, currentStamina));
            regenCoroutine = StartCoroutine(RegenerateStamina());
        }
    }

    public void ResetMaxStamina(float newMaxStamina)
    {
        var oldMaxStamina = maxStamina;
        maxStamina = newMaxStamina;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
        changeMaxStaminaEvent.Raise(this, (maxStamina, oldMaxStamina));
    }

    private IEnumerator RegenerateStamina()
    {
        isRegenerating = false;
        yield return new WaitForSeconds(regenDelay);
        isRegenerating = true;

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            changeStaminaEvent.Raise(this, (maxStamina, currentStamina));
            yield return null;
        }
    }
}
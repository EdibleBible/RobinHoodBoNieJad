using System;
using Script.ScriptableObjects;
using UnityEngine;

public class PlayerTorchSystem : MonoBehaviour
{
    [SerializeField] private float baseMaxTorchFuel;
    private float maxTorchFuel;
    [SerializeField] private SOPlayerStatsController playerStatsController;
    [SerializeField] private GameEvent changeTorchFuelEvent;
    [SerializeField] private GameEvent changeMaxTorchFuelEvent;
    private bool isTorchOn = false;
    [SerializeField] private float torchFuelUse;
    [HideInInspector] public float currentTorchFuel;
    private PlayerAnimatorController playerAnimatorController;

    [SerializeField] private GameObject torchObject;
    private void Start()
    {
        SetupTorchFuel(true);
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        
    }

    private void Update()
    {
        if (isTorchOn && torchObject.activeInHierarchy)
            UseFueal(torchFuelUse);

        if (currentTorchFuel <= 0)
            ToogleTorch(false);
    }

    public void SetupTorchFuel(bool resetCurrentTorchFuel = false)
    {
        var oldMaxFuel = maxTorchFuel;
        maxTorchFuel = (baseMaxTorchFuel +
                        (int)Math.Floor(playerStatsController.GetSOPlayerStats(E_ModifiersType.Fuel).Additive)) *
                       (int)Math.Floor(playerStatsController.GetSOPlayerStats(E_ModifiersType.Fuel).Multiplicative);

        if(oldMaxFuel == 0)
            oldMaxFuel = maxTorchFuel;
        
        if (resetCurrentTorchFuel)
            currentTorchFuel = maxTorchFuel;

        changeMaxTorchFuelEvent.Raise(this, (maxTorchFuel, oldMaxFuel));
        changeTorchFuelEvent.Raise(this, (maxTorchFuel, currentTorchFuel));
    }

    public void UseFueal(float perSec)
    {
        currentTorchFuel -= perSec * Time.deltaTime;
        currentTorchFuel = Mathf.Clamp(currentTorchFuel, 0, maxTorchFuel);


        changeTorchFuelEvent.Raise(this, (maxTorchFuel, currentTorchFuel));
    }

    public void ResetMaxFuel(float newMaxStamina)
    {
        var oldMaxFuel = maxTorchFuel;
        maxTorchFuel = newMaxStamina;
        currentTorchFuel = Mathf.Min(currentTorchFuel, maxTorchFuel);
        changeMaxTorchFuelEvent.Raise(this, (maxTorchFuel, oldMaxFuel));
    }

    public void AddFuel(float amount)
    {
        currentTorchFuel += amount;
        currentTorchFuel = Mathf.Clamp(currentTorchFuel, 0, maxTorchFuel);
        changeTorchFuelEvent.Raise(this, (maxTorchFuel, currentTorchFuel));
    }

    public void ToogleTorch(bool toogleTo)
    {
        isTorchOn = toogleTo;
        if(!isTorchOn)
            StopGlowingTorch();
    }
    
    public void ToogleTorch()
    {
        isTorchOn = !isTorchOn;
        if(!isTorchOn)
            StopGlowingTorch();
        
        playerAnimatorController.ToogleTorch(isTorchOn);
        

    }

    public void StartGlowingTorch()
    {
        torchObject.SetActive(true);
    }

    public void StopGlowingTorch()
    {
        torchObject.SetActive(false);
    }
}
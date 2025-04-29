using UnityEngine;

public class TorchFuelItem : ItemBase
{
    [SerializeField]
    private float fuelAmount;
    public override void Interact(Transform player)
    {
        if (player.TryGetComponent(out PlayerTorchSystem staminaSystem))
        {
            staminaSystem.AddFuel(fuelAmount);
            Destroy(gameObject);
        }
    }
}

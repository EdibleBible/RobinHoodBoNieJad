using System;
using System.Linq;
using Script.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBase : MonoBehaviour
{
    public Camera camera;

    [SerializeField] private Transform dropPointTransform;
    public SOInventory PlayerInventory;
    [SerializeField] private GameEvent InventorySetUpEvent;
    [SerializeField] private GameEvent DropItemEvent;
    [SerializeField] private GameEvent PickupItemEvent;
    [SerializeField] private GameEvent InventoryUpdateSelectedItemEvent;
    public int currentSelectedItem = 0;

    public SOPlayerStatsController PlayerStatsController;

    [HideInInspector] public ItemData CurrSelectedItem = null;
    private PlayerStaminaSystem playerStaminaSystem;
    private PlayerTorchSystem playerTorchSystem;

    private void Awake()
    {
        playerStaminaSystem = GetComponent<PlayerStaminaSystem>();
        playerTorchSystem = GetComponent<PlayerTorchSystem>();
    }

    public void Start()
    {
        PlayerInventory.ClearInventory();
        PlayerInventory.SetUpInventory();
        PlayerStatsController.SetPlayerBaseModifiers();
        ResetInventory();
        InventoryUpdateSelectedItemEvent?.Raise(this, (0, 0));
    }

    private void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
        {
            UpdateSelectedSlot(-1); // Scroll w górę
        }
        else if (scrollInput < 0f)
        {
            UpdateSelectedSlot(1); // Scroll w dół
        }
    }

    private void UpdateSelectedSlot(int direction)
    {
        if (currentSelectedItem + direction < 0 ||
            currentSelectedItem + direction >= PlayerInventory.CurrInventorySize) return;
        (int curr, int prev) data = (currentSelectedItem + direction, currentSelectedItem);
        currentSelectedItem = data.curr;
        InventoryUpdateSelectedItemEvent?.Raise(this, data);
    }

    public bool PickUp(ItemData itemBase)
    {
        if (PlayerInventory.AddItemToInventory(itemBase))
        {
            PickupItemEvent?.Raise(this, itemBase);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DropItem()
    {
        if (CurrSelectedItem != null)
        {
            var obj = Instantiate(CurrSelectedItem.ItemPrefab, dropPointTransform.position, quaternion.identity);

            DropSound dropSound = obj.GetComponent<DropSound>();
            if (dropSound == null)
            {
                Debug.LogError("DropSound component not found on the instantiated object: " + obj.name);
            }
            else
            {
               // dropSound.materialTypeDrop = (DropSound.MaterialTypeDrop)GetDropMaterialParameter(CurrSelectedItem.ItemType);
                
                dropSound.PlayDropSound();
            }


            obj.transform.rotation = quaternion.identity;
            PlayerInventory.RemoveItemFromInventory(CurrSelectedItem);
            CurrSelectedItem.RemoveModifier(PlayerStatsController);

            if (CurrSelectedItem.StatsToChange.Any(x => x.ModifierType == E_ModifiersType.Inventory))
                ResetInventory();
            }

            if (CurrSelectedItem.StatsToChange.Any(x => x.ModifierType == E_ModifiersType.Stamina))
                ResetStamina();
            }

            if (CurrSelectedItem.StatsToChange.Any(x => x.ModifierType == E_ModifiersType.Fuel))
            {
                ResetFuel();
            }

            DropItemEvent?.Raise(this, currentSelectedItem);
        }
    }


    public void RemoveItemFromInventory(ItemData itemToRemove)
    {
        PlayerInventory.RemoveItemFromInventory(itemToRemove);
        DropItemEvent?.Raise(this, itemToRemove);
    }

    public void ResetInventory()
    {
        PlayerInventory.CalculateItemsSlotsCount();
        InventorySetUpEvent?.Raise(this, PlayerInventory);
    }

    public void ResetStamina()
    {
        playerStaminaSystem.SetUpStamina();
    }

    public void ResetFuel()
    {
        playerTorchSystem.SetupTorchFuel();
    }

    private void PlayDropSound(ItemType itemType)
    {
        var instance = FMODUnity.RuntimeManager.CreateInstance("event:/ItemDrop");
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(dropPointTransform.position));
        instance.setParameterByName("MaterialTypeDrop", GetDropMaterialParameter(itemType));
        instance.start();
        instance.release();
    }

    private int GetDropMaterialParameter(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.SteelShoes => 0, // Cloth
            ItemType.CollectibleVase => 1, // Porcelain
            ItemType.CollectibleBox => 2, // Wood
            ItemType.CollectibleGoblet => 3, // Glass
            ItemType.Hammer => 4,
            ItemType.Key => 5,
            _ => 0
        };
    }

}
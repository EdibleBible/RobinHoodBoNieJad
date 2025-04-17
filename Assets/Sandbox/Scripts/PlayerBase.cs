using System;
using System.Linq;
using Script.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private PlayerWalk playerWalk;
    private PlayerInteractionController playerInteractionController;

    private PlayerControll PlayerInputActions => InputManager.Instance.PlayerInputActions;

    private void Start()
    {
        playerStaminaSystem = GetComponent<PlayerStaminaSystem>();
        playerTorchSystem = GetComponent<PlayerTorchSystem>();
        playerWalk = GetComponent<PlayerWalk>();
        playerInteractionController = GetComponent<PlayerInteractionController>();

        // Input Hook
        PlayerInputActions.Player.Movement.performed += Movement_Performed;
        PlayerInputActions.Player.Movement.canceled += Movement_Canceled;

        PlayerInputActions.Player.Interaction.performed += OnInteraction_Performed;
        PlayerInputActions.Player.CancelInteraction.performed += OnCancellInteraction_Performed;

        PlayerInputActions.Player.Sprint.performed += OnSprint_Performed;
        PlayerInputActions.Player.Sprint.canceled += OnSprint_Canceled;

        PlayerInputActions.Player.Crouch.performed += OnCrouch_Performed;
        PlayerInputActions.Player.Crouch.canceled += OnCrouch_Canceled;

        PlayerInputActions.Player.ToogleTorch.performed += ToogleTorch_Performed;

        PlayerInputActions.Player.DropInventory.performed += OnDropItem_Performed;

        PlayerInputActions.Player.ChangeItemPositive.performed += OnChangeIntem_Performed;
        
        PlayerInventory.ClearInventory();
        PlayerInventory.SetUpInventory();
        PlayerStatsController.SetPlayerBaseModifiers();
        ResetInventory();
        InventoryUpdateSelectedItemEvent?.Raise(this, (0, 0));
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
            var obj = Instantiate(CurrSelectedItem.ItemPrefab);

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

            obj.transform.position = dropPointTransform.position;
            obj.transform.rotation = quaternion.identity;
            PlayerInventory.RemoveItemFromInventory(CurrSelectedItem);
            CurrSelectedItem.RemoveModifier(PlayerStatsController);

            if (CurrSelectedItem.StatsToChange.Any(x => x.ModifierType == E_ModifiersType.Inventory))
            {
                ResetInventory();
            }

            if (CurrSelectedItem.StatsToChange.Any(x => x.ModifierType == E_ModifiersType.Stamina))
            {
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

    public void Movement_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Chuj");
        var parameters = context.ReadValue<Vector2>();
        playerWalk.SetAxisMovement(parameters);
    }

    public void Movement_Canceled(InputAction.CallbackContext context)
    {
        playerWalk.SetAxisMovement(Vector2.zero);
    }

    public void OnInteraction_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Interaction performed");
        playerInteractionController.Interact();
    }

    public void OnCancellInteraction_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Cancell Interaction performed");
        playerInteractionController.StopInteracting();
    }

    public void OnCrouch_Performed(InputAction.CallbackContext context)
    {
        playerWalk.Crouching = true;
    }

    public void OnCrouch_Canceled(InputAction.CallbackContext context)
    {
        playerWalk.Crouching = false;
    }

    public void OnSprint_Performed(InputAction.CallbackContext context)
    {
        playerWalk.Sprinting = true;
    }

    private void OnSprint_Canceled(InputAction.CallbackContext obj)
    {
        playerWalk.Sprinting = false;
    }

    public void ToogleTorch_Performed(InputAction.CallbackContext context)
    {
        playerTorchSystem.ToogleTorch();
    }

    public void OnDropItem_Performed(InputAction.CallbackContext context)
    {
        DropItem();
    }

    public void OnChangeIntem_Performed(InputAction.CallbackContext context)
    {
        float axis = context.ReadValue<float>();


        if (axis > 0f)
        {
            UpdateSelectedSlot(-1); // Scroll w górę
        }
        else if (axis < 0f)
        {
            UpdateSelectedSlot(1); // Scroll w dół
        }
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
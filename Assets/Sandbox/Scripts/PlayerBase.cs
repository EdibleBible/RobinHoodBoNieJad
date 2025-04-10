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
    
    PlayerInput playerInput;

    private void Awake()
    {
        playerStaminaSystem = GetComponent<PlayerStaminaSystem>();
        playerTorchSystem = GetComponent<PlayerTorchSystem>();
        playerWalk = GetComponent<PlayerWalk>();

        PlayerControll playerInputActions = new PlayerControll();
        playerInputActions.Player.Enable();

        
        playerInputActions.Player.Movement.performed += Movement_Performed;
        playerInputActions.Player.Movement.canceled += Movement_Canceled;
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
            var obj = Instantiate(CurrSelectedItem.ItemPrefab);
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
        var parameters = context.ReadValue<Vector2>();
        playerWalk.SetAxisMovement(parameters);
    }
    
    public void Movement_Canceled(InputAction.CallbackContext context)
    {
        playerWalk.SetAxisMovement(Vector2.zero);
    }
    
    public void OnCrouch_Performed(InputAction.CallbackContext context)
    {
        
    }

    public void OnSprint_Performed(InputAction.CallbackContext context)
    {
        
    }

    public void ToogleTorch_Performed(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteraction_Performed(InputAction.CallbackContext context)
    {
        
    }

    public void OnCancellInteraction_Performed(InputAction.CallbackContext context)
    {
        
    }

    public void OnDropItem_Performed(InputAction.CallbackContext context)
    {
        
    }

    public void OnChangeIntem_Performed(InputAction.CallbackContext context)
    {
        
    }
}
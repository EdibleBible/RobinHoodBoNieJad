using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private SOInventory playerInventory;
    /*public PlayerInputInteract interact;
    public PlayerHotbar hotbar;
    public SOInventory inventory;
    public Vector3 position { get { return transform.position; } }
    public int hotbarSize {
        get { return hotbar.size; }
        set { hotbar.Resize(value); }
    }

    private void Awake()
    {
        interact.playerBase = this;
        hotbar.playerBase = this;
    }

    /*private void Start()
    {
        hotbar.LoadFromInventory(inventory);
    }#1#

    private void OnEnable()
    {
        MenuCheats.GetPlayerBase += ReturnThis;
    }

    PlayerBase ReturnThis()
    {
        MenuCheats.GetPlayerBase -= ReturnThis;
        return this;
    }

    public bool PickUp(ItemBase item)
    {
        return hotbar.PickUp(item);
    }*/
    public bool PickUp(ItemBase itemBase)
    {
        return playerInventory.AddItemToInventory(itemBase);
    }
}

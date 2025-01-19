using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public PlayerInputInteract interact;
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

    private void Start()
    {
        hotbar.SaveToInventory(inventory);
    }

    public bool PickUp(ItemBase item)
    {
        return hotbar.PickUp(item);
    }
}

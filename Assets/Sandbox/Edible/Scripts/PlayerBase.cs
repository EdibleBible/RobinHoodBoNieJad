using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public PlayerInputInteract interact;
    public PlayerHotbar hotbar;
    public Vector3 position { get { return transform.position; } }
    public int hotbarSize {   
        get { return hotbar.size; }    
        set { hotbar.Resize(value); } 
    }

    private void Awake()
    {
        hotbar.playerBase = this;
    }

    public bool PickUp(ItemBase item)
    {
        return hotbar.PickUp(item);
    }
}

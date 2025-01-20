using UnityEngine;

public interface IInteract
{
    public bool Interact(PlayerBase playerBase);
    GameObject gameObject { get; }
}

using UnityEngine;

public interface IInteractable
{
    GameEvent ShowUIEvent { get; set; }
    GameEvent InteractEvent { get; set; }
    bool CanInteract { get; set; }
    bool IsBlocked { get; set; }
    bool TwoSideInteraction { get; set; }
    string InteractMessage { get; set; }
    
    string BlockedMessage { get; set; }

    void Interact(Transform player);
    void ShowUI();
    void HideUI();
    
    
}

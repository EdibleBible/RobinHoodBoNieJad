using System;
using UnityEngine;

public class GameoverController : MonoBehaviour
{
    public GameEvent LoseGameEvent;
    [SerializeField] private SOInventory _inventory;

    public void LoseGame()
    {
        _inventory.ClearInventory();
        LoseGameEvent?.Raise(this,null);
    }
}

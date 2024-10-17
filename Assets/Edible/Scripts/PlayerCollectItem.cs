using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollectItem : MonoBehaviour
{
    private PlayerInventory inventoryScript;

    private void Awake()
    {
        inventoryScript = GetComponent<PlayerInventory>();
    }

    public bool AttemptCollect(Item itemScript)
    {
        if (inventoryScript.AttemptAddToInventory(itemScript))
        {
            return true;
        }
        return false;
    }

    public void Collect(Item itemScript)
    {
        inventoryScript.AddToInventory(itemScript);
    }
}

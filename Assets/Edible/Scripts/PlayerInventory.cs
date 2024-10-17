using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Item> itemList = new();
    public int inventoryLimit = 3;

    public bool AttemptAddToInventory(Item itemScript)
    {
        if (itemList.Count >= inventoryLimit)
        {
            return false;
        }
        return true;
    }

    public void AddToInventory(Item itemScript)
    {
        itemList.Add(itemScript);
        Debug.Log("Added " + itemScript.gameObject.name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleGetCollected : MonoBehaviour
{
    public GameObject itemObject;
    private Item itemScript;
    private PlayerCollectItem playerScript;

    private void Awake()
    {
        itemScript = itemObject.GetComponent<Item>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerScript = other.GetComponent<PlayerCollectItem>();
            if (playerScript.AttemptCollect(itemScript))
            {
                playerScript.Collect(itemScript);
            }
        }
    }
}

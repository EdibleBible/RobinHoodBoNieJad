using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class ToogleObject : MonoBehaviour
{
    private IInteractable interactable;
    public SerializedDictionary<GameObject,bool> toogleObjects = new SerializedDictionary<GameObject, bool>();

    private void Awake()
    {
        interactable = gameObject.GetComponent<IInteractable>();
    }

    public void Toogle(Component sender, object data)
    {
        if (sender is IInteractable interactable && interactable == this.interactable)
        {
            foreach (var obj in toogleObjects)
            {
                obj.Key.SetActive(obj.Value);
            }
        }
    }
}
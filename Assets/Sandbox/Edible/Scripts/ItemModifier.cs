using System;
using UnityEngine;

public class ItemModifier : MonoBehaviour
{
    [Header("Attributes")]
    public string Name;
    public float Value;
    public Sprite Icon;
    [Header("Unused Attributes")]
    public string Description;
    public int InventorySize;
    public GameObject ObjectPrefab;

    [Header("Modifiers")]
    public int playerInventorySize;
}
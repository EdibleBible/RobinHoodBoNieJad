using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour
{
    [Header("UI References")]
    public Transform listParent; // Parent Transform for the list items
    public GameObject listItemPrefab; // Prefab for a single list item

    [Header("Data List")]
    public List<ItemBase> items = new List<ItemBase>();
    public List<ListItemData> itemList = new List<ListItemData>(); // Data list

    private void Start()
    {
    }

    public void AddNewItem(string name, Sprite icon)
    {
        itemList.Clear();
        ListItemData newItem;

        foreach (var item in items)
        {
            newItem = new ListItemData { itemName = item.itemName, itemIcon = item.itemIcon };
            itemList.Add(newItem);
        }

        // Update the UI
        UpdateUIList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UpdateUIList();
        }
    }

    private void UpdateUIList()
    {
        // Clear old UI items
        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

        // Create new UI items for each entry in the list
        foreach (var item in itemList)
        {
            // Instantiate a new list item
            GameObject newItem = Instantiate(listItemPrefab, listParent);

            // Find and set the icon
            Image iconImage = newItem.transform.Find("Icon").GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = item.itemIcon;
            }

            // Find and set the name text
            Text nameText = newItem.transform.Find("Name").GetComponent<Text>();
            if (nameText != null)
            {
                nameText.text = item.itemName;
            }
        }
    }
}

[System.Serializable]
public class ListItemData
{
    public string itemName; // Name of the item
    public Sprite itemIcon; // Icon of the item
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionCell : MonoBehaviour
{
    public bool debugTrue;
    public SOInventory stats;
    public ItemBase item;
    public Button button;
    public Image image;
    public GameObject infoBanner;
    public Image infoImage;
    public TMP_Text infoName;
    public TMP_Text infoDescription;
    private ItemData itemData;
    private ItemType type;
    private int id;

    private void Awake()
    {
        itemData = item.ItemData;
        type = item.ItemData.ItemType;
        id = item.ItemData.CollectibleId;
        button.interactable = false;
    }

    private void OnEnable()
    {
        image.sprite = itemData.ItemIcon;
        foreach (var invItem in stats.InventoryLobby)
        {
            if (invItem.ItemType == type && invItem.CollectibleId == id || debugTrue)
            {
                button.interactable = true;
                if (GetCollectionState(id))
                {
                    image.color = Color.white;
                }
            }
        }
    }

    public void SelectItem()
    {
        if (!GetCollectionState(id))
        {
            CollectItem();
        }
        infoBanner.SetActive(true);
        infoImage.sprite = itemData.ItemIcon;
        infoName.text = itemData.ItemName;
        infoDescription.text = itemData.ItemDescription;
    }

    private void CollectItem()
    {
        SetCollectionState(id);
        image.color = Color.white;
        foreach (var invItem in stats.InventoryLobby)
        {
            if (invItem.ItemType == type && invItem.CollectibleId == id)
            {
                stats.InventoryLobby.Remove(invItem);
                return;
            }
        }
    }

    private bool GetCollectionState(int id)
    {
        switch (type)
        {
            case ItemType.CollectibleGoblet:
                return stats.IsCollectedGoblet(id);
            case ItemType.CollectibleVase:
                return stats.IsCollectedVase(id);
            case ItemType.CollectibleBook:
                return stats.IsCollectedBook(id);
            default:
                return false;
        }
    }

    private void SetCollectionState(int id)
    {
        switch (type)
        {
            case ItemType.CollectibleGoblet:
                stats.CollectGoblet(id);
                break;
            case ItemType.CollectibleVase:
                stats.CollectVase(id);
                break;
            case ItemType.CollectibleBook:
                stats.CollectBook(id);
                break;
            default:
                break;
        }
    }
}

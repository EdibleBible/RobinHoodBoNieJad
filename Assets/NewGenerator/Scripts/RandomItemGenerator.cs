using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Vector3 = System.Numerics.Vector3;

public class RandomItemGenerator : MonoBehaviour
{
    public SOPlayerQuest currentQuest;
    public List<ItemBase> ItemsDatabase = new List<ItemBase>();
    public SerializedDictionary<ItemType, bool> isItemAvaible = new SerializedDictionary<ItemType, bool>();

    public void ControllSpawner()
    {
        foreach (var item in currentQuest.RequireItems)
        {
            isItemAvaible.Add(item.Key, false);
        }

        Randomize();
    }

    public void Randomize()
    {
        if (CheckQuestIsAvaible())
        {
            SpawnRandomItem();
        }
        else
        {
            SpawnQuestItem();
        }
    }

    private void SpawnQuestItem()
    {
        List<ItemType> availableItems = isItemAvaible.Where(x => !x.Value).Select(x => x.Key).ToList();
        Debug.Log($"[SpawnQuestItem] Available quest item types: {string.Join(", ", availableItems)}");

        if (availableItems.Count == 0)
        {
            Debug.Log("[SpawnQuestItem] No available quest items left. Spawning random item.");
            SpawnRandomItem();
            return;
        }

        int index = Random.Range(0, availableItems.Count);
        ItemType itemTypeToSpawn = availableItems[index];
        Debug.Log($"[SpawnQuestItem] Selected item type to spawn: {itemTypeToSpawn}");

        List<ItemBase> itemsAvaibleToSpawn = ItemsDatabase.Where(x => x.ItemData.ItemType == itemTypeToSpawn).ToList();
        Debug.Log($"[SpawnQuestItem] Found {itemsAvaibleToSpawn.Count} items in database for type {itemTypeToSpawn}");

        if (itemsAvaibleToSpawn.Count <= 0)
        {
            Debug.LogWarning($"[SpawnQuestItem] No items to spawn for type {itemTypeToSpawn}");
            return;
        }

        int newIndex = Random.Range(0, itemsAvaibleToSpawn.Count);
        ItemBase item = itemsAvaibleToSpawn[newIndex];
        Debug.Log($"[SpawnQuestItem] Chosen item prefab index: {newIndex}, name: {item.name}");

        if (item.ItemData.ItemType == ItemType.CollectibleGoblet)
        {
            List<ItemBase> allGoblet = Resources.LoadAll<ItemBase>("Pickable/Goblet").ToList();
            item = allGoblet[Random.Range(0, allGoblet.Count)];
            Debug.Log($"[SpawnQuestItem] Replaced with random goblet prefab: {item.name}");
        }
        else if (item.ItemData.ItemType == ItemType.CollectibleVase)
        {
            List<ItemBase> allVase = Resources.LoadAll<ItemBase>("Pickable/Vase").ToList();
            item = allVase[Random.Range(0, allVase.Count)];
            Debug.Log($"[SpawnQuestItem] Replaced with random vase prefab: {item.name}");
        }
        else if (item.ItemData.ItemType == ItemType.CollectibleBook)
        {
            List<ItemBase> allBook = Resources.LoadAll<ItemBase>("Pickable/Book").ToList();
            item = allBook[Random.Range(0, allBook.Count)];
            Debug.Log($"[SpawnQuestItem] Replaced with random book prefab: {item.name}");
        }

        if (!currentQuest.SpawnedItems.ContainsKey(itemTypeToSpawn))
        {
            currentQuest.SpawnedItems[itemTypeToSpawn] = 0;
        }

        currentQuest.SpawnedItems[itemTypeToSpawn]++;
        Debug.Log(
            $"[SpawnQuestItem] Spawned count for {itemTypeToSpawn}: {currentQuest.SpawnedItems[itemTypeToSpawn]} / {currentQuest.RequireItems[itemTypeToSpawn].RequiredAmount}");

        if (currentQuest.SpawnedItems[itemTypeToSpawn] >= currentQuest.RequireItems[itemTypeToSpawn].RequiredAmount)
        {
            isItemAvaible[itemTypeToSpawn] = true;
            Debug.Log($"[SpawnQuestItem] Required amount reached for {itemTypeToSpawn}. Marking as collected.");
        }

        var obj = Instantiate(item, transform);
        obj.transform.SetParent(null);
        Debug.Log($"[SpawnQuestItem] Spawned object: {obj.name} at position {obj.transform.position}");
    }


    private void SpawnRandomItem()
    {
        int randomIndex = Random.Range(0, ItemsDatabase.Count);

        ItemBase item = ItemsDatabase[randomIndex];

        if (item.ItemData.ItemType == ItemType.CollectibleGoblet)
        {
            List<ItemBase> allGoblet = Resources.LoadAll<ItemBase>("Pickable/Goblet").ToList();
            item = allGoblet[Random.Range(0, allGoblet.Count)];
        }
        else if (item.ItemData.ItemType == ItemType.CollectibleVase)
        {
            List<ItemBase> allVase = Resources.LoadAll<ItemBase>("Pickable/Vase").ToList();
            item = allVase[Random.Range(0, allVase.Count)];
        }
        else if (item.ItemData.ItemType == ItemType.CollectibleBook)
        {
            List<ItemBase> allBook = Resources.LoadAll<ItemBase>("Pickable/Book").ToList();
            item = allBook[Random.Range(0, allBook.Count)];
        }

        var obj = Instantiate(item, transform);
        obj.transform.localPosition = new UnityEngine.Vector3(0, 0, 0);
        obj.transform.SetParent(null);
    }

    private bool CheckQuestIsAvaible()
    {
        List<ItemType> itemsToMarkAsCompleted = new List<ItemType>();

        foreach (var item in isItemAvaible)
        {
            if (currentQuest.SpawnedItems.ContainsKey(item.Key))
            {
                if (currentQuest.RequireItems[item.Key].RequiredAmount <= currentQuest.SpawnedItems[item.Key])
                {
                    itemsToMarkAsCompleted.Add(item.Key);
                    continue;
                }
            }
            else
            {
                currentQuest.SpawnedItems.Add(item.Key, 0);
            }

            if (!item.Value)
            {
                return false;
            }
        }

        // Po foreach ustawiamy wartości
        foreach (var key in itemsToMarkAsCompleted)
        {
            isItemAvaible[key] = true;
        }

        return true;
    }


#if UNITY_EDITOR
    private void OnDisable()
    {
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            currentQuest.ResetSpawnedItem();
        }
    }
#endif
    public void SetupSpawner(GameController instance)
    {
        if (instance.AllPlayerQuest.CurrentSelectedQuest != null)
            currentQuest = instance.AllPlayerQuest.CurrentSelectedQuest;
        else
        {
            instance.AllPlayerQuest.RandomizeAllQuests();
            instance.AllPlayerQuest.CurrentSelectedQuest = instance.AllPlayerQuest.randomizedQuests[0];
        }
    }
}
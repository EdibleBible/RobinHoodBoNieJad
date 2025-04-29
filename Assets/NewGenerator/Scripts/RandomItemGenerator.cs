using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

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
        if (availableItems.Count == 0)
        {
            SpawnRandomItem();
            return;
        }

        int index = Random.Range(0, availableItems.Count);
        ItemType itemTypeToSpawn = availableItems[index];

        List<ItemBase> itemsAvaibleToSpawn = ItemsDatabase.Where(x => x.ItemData.ItemType == itemTypeToSpawn).ToList();
        int newIndex = Random.Range(0, itemsAvaibleToSpawn.Count);

        if (itemsAvaibleToSpawn.Count <= 0)
            return;

        Debug.Log(newIndex);
        ItemBase item = itemsAvaibleToSpawn[newIndex];

        if (!currentQuest.SpawnedItems.ContainsKey(itemTypeToSpawn))
        {
            currentQuest.SpawnedItems[itemTypeToSpawn] = 0;
        }

        currentQuest.SpawnedItems[itemTypeToSpawn]++;

        // DODAJ TO: Jeśli osiągnęliśmy wymagany amount, oznacz jako zebrany
        if (currentQuest.SpawnedItems[itemTypeToSpawn] >= currentQuest.RequireItems[itemTypeToSpawn].RequiredAmount)
        {
            isItemAvaible[itemTypeToSpawn] = true;
        }

        var obj = Instantiate(item, transform);
        obj.transform.SetParent(null);
    }

    private void SpawnRandomItem()
    {
        int randomIndex = Random.Range(0, ItemsDatabase.Count);
        var obj = Instantiate(ItemsDatabase[randomIndex], transform);
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
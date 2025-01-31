using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectRandomSpawner : MonoBehaviour
{
    public SOLevel levelData;
    public float seedMod = 100;
    [Range(0,100)] public int itemModChance;
    private int seed;
    public List<GameObject> objects = new();
    private GameObject spawnedObject;

    private void OnEnable()
    {
        seed = levelData.levelSeed;
        if (objects.Count != 0)
        {
            int x = (int)(transform.position.x * seedMod);
            int y = (int)(transform.position.y * seedMod);
            int z = (int)(transform.position.z * seedMod);
            spawnedObject = objects[(seed * x * y * z) % objects.Count];
            spawnedObject = Instantiate(spawnedObject, transform.position,transform.rotation, gameObject.transform.parent);
        }
        if (spawnedObject.TryGetComponent<ItemBase>(out ItemBase item))
        {
            if (Random.Range(0, 100) <= itemModChance)
            {
                item.itemValue += Random.Range(10, 20);
                item.itemValue = item.itemValue * Random.Range(1, 2);
                item.itemName = "Enchanted " + item.itemName;
            }
        }
    }
}

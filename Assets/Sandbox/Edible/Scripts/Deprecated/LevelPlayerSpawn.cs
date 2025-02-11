using UnityEngine;

public class LevelPlayerSpawn : MonoBehaviour
{
    public bool isEnabled;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (isEnabled)
        {
            Instantiate(playerPrefab);
        }
    }
}

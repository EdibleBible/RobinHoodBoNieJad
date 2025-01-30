using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectLevelExit : MonoBehaviour, IUseObject
{
    private PlayerBase player;
    private bool isPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            player = other.transform.parent.GetComponent<PlayerBase>();
            isPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            isPlayer = false;
        }
    }

    public void UseObject()
    {
        if (isPlayer &&  player != null)
        {
            player.hotbar.SaveToInventory(player.inventory);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(1);
        }
    }
}

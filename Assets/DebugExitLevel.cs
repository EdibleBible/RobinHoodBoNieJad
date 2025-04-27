using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugExitLevel : MonoBehaviour
{
    [SerializeField] private SOPlayerQuest playerQuest;
    [SerializeField] private SOAllQuest allQuest;
    private void Awake()
    {
        playerQuest = GameController.Instance.AllPlayerQuest.CurrentSelectedQuest;
        allQuest = GameController.Instance.AllPlayerQuest;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SoundManager.Instance?.StopAllActiveEvents();
            var emitters = FindObjectsByType<FMODUnity.StudioEventEmitter>(FindObjectsSortMode.None);
            foreach (var emitter in emitters)
            {
                if (emitter.IsPlaying())
                {
                    emitter.Stop();
                }
            }
        
            allQuest.RandomizeSelectedQuest(playerQuest.Difficulty,false,playerQuest);
        
            var player = FindObjectOfType<PlayerBase>();
            if (player != null)
            {
                Destroy(player.gameObject);
            }
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(1);
        }
        else if(Input.GetKeyDown(KeyCode.L))
        {
            SoundManager.Instance?.StopAllActiveEvents();
            var emitters = FindObjectsByType<FMODUnity.StudioEventEmitter>(FindObjectsSortMode.None);
            foreach (var emitter in emitters)
            {
                if (emitter.IsPlaying())
                {
                    emitter.Stop();
                }
            }
        
            allQuest.RandomizeSelectedQuest(playerQuest.Difficulty,true,playerQuest);
        
            var player = FindObjectOfType<PlayerBase>();
            if (player != null)
            {
                Destroy(player.gameObject);
            }
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(1);
        }
        else if(Input.GetKeyDown(KeyCode.O))
        {
            SoundManager.Instance?.StopAllActiveEvents();
            var emitters = FindObjectsByType<FMODUnity.StudioEventEmitter>(FindObjectsSortMode.None);
            foreach (var emitter in emitters)
            {
                if (emitter.IsPlaying())
                {
                    emitter.Stop();
                }
            }
        
            allQuest.RandomizeSelectedQuest(playerQuest.Difficulty,playerQuest.IsQuestComplete(),playerQuest);
        
            var player = FindObjectOfType<PlayerBase>();
            if (player != null)
            {
                Destroy(player.gameObject);
            }
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(1);
        }
    }
}

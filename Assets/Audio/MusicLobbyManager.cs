using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private EventInstance musicInstance;
    private static MusicManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // W razie gdyby scena Lobby by³a uruchamiana jako pierwsza
        Scene currentScene = SceneManager.GetActiveScene();
        HandleSceneMusic(currentScene.name);
    }

    void StartMusic()
    {
        if (!musicInstance.isValid())
        {
            musicInstance = RuntimeManager.CreateInstance("event:/MusicLobby");
            musicInstance.start();
        }
    }

    void StopMusic()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneMusic(scene.name);
    }

    void HandleSceneMusic(string sceneName)
    {
        if (sceneName == "Lobby")
        {
            StartMusic();
        }
        else
        {
            StopMusic(); // W ka¿dej innej scenie muzyka znika
        }
    }
}

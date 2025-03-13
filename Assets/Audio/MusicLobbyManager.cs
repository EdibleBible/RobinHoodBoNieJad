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

    void StartMusic()
    {
        if (!musicInstance.isValid()) // Unikamy podw�jnego uruchamiania
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
        if (scene.name == "MainMenu")
        {
            StopMusic(); // Wy��cz muzyk� w MainMenu
        }
        else if (scene.name == "Lobby")
        {
            StartMusic(); // W��cz muzyk� w Lobby
        }

        if (scene.name == "Diorama")
        {
            StopMusic(); // Wy��cz muzyk� w MainMenu
        }
        else if (scene.name == "Lobby")
        {
            StartMusic(); // W��cz muzyk� w Lobby
        }
    }
}

using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class FMODBusStopper : MonoBehaviour
{
    private Bus sfxBus;

    void Awake()
    {
        // Inicjalizacja busa
        sfxBus = RuntimeManager.GetBus("bus:/SFX");

        // Subskrybuj za³adowanie sceny
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Opcjonalnie nie niszcz obiektu miêdzy scenami
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LobbyScene")
        {
            StopSFXBus();
        }
    }

    private void StopSFXBus()
    {
        if (sfxBus.isValid())
        {
            sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

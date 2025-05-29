using System.Collections.Generic;
using FMOD.Studio;          // <- potrzebne do EventInstance + STOP_MODE
using FMODUnity;           // <- RuntimeManager, StudioEventEmitter
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private readonly List<EventInstance> activeEvents = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /* ------------------------------------------------------------ */
    /*  TWORZENIE I REJESTRACJA EVENTÓW                             */
    /* ------------------------------------------------------------ */
    public EventInstance CreateAndStartEvent(string path)
    {
        EventInstance instance = RuntimeManager.CreateInstance(path);
        instance.start();
        activeEvents.Add(instance);
        return instance;
    }

    /* ------------------------------------------------------------ */
    /*  ZATRZYMANIE ABSOLUTNIE WSZYSTKIEGO W OKREŒLONYCH SCENACH    */
    /* ------------------------------------------------------------ */
    public void StopAllActiveEvents()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "NewGeneratorScene" && currentScene != "TutorialScene")
            return;

        /* 1) EventInstance’y tworzone rêcznie ------------------------ */
        foreach (var e in activeEvents)
        {
            e.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            e.release();
        }
        activeEvents.Clear();

        /* 2) Wszystkie StudioEventEmitter’y w hierarchii ------------- */
#if UNITY_2023_1_OR_NEWER
        var emitters = Object.FindObjectsByType<StudioEventEmitter>(
            UnityEngine.FindObjectsInactive.Include,
            UnityEngine.FindObjectsSortMode.None);
#else
    var emitters = Object.FindObjectsByType<StudioEventEmitter>(
        UnityEngine.FindObjectsSortMode.None, true);
#endif
        foreach (var emitter in emitters)
            emitter.Stop();

        /* 3) Ostateczne wyciszenie przez master bus ------------------ */
        RuntimeManager.StudioSystem.flushCommands();          // dopchnij komendy
        var masterBus = RuntimeManager.GetBus("bus:/");       // g³ówny bus
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}

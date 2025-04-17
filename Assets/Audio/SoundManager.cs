using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private List<EventInstance> activeEvents = new List<EventInstance>();

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

    public EventInstance CreateAndStartEvent(string path)
    {
        EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(path);
        instance.start();
        activeEvents.Add(instance);
        return instance;
    }

    public void StopAllActiveEvents()
    {
        foreach (var instance in activeEvents)
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }
        activeEvents.Clear();
    }
}

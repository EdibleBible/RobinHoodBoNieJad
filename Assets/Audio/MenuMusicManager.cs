using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MenuMusicManager : MonoBehaviour
{
    public EventReference musicEvent; // Przypisz event FMOD w Inspectorze
    private EventInstance musicInstance;
    private int currentTrack = 0; // 0 - pierwszy utwór, 1 - drugi

    void Start()
    {
        // Tworzymy instancjê eventu muzyki
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

    public void SwitchTrack()
    {
        currentTrack = 1 - currentTrack; // Zmiana utworu (0 -> 1, 1 -> 0)
        musicInstance.setParameterByName("TrackSelect", currentTrack);
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}

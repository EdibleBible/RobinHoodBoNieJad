using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class VCASystem : MonoBehaviour
{
    public Slider Master;
    public Slider Music;
    public Slider SFX;
    public Slider UI;

    private VCA masterVCA;
    private VCA musicVCA;
    private VCA sfxVCA;
    private VCA uiVCA;

    private void Start()
    {
        // Pobranie VCA z FMOD
        masterVCA = RuntimeManager.GetVCA("vca:/MasterVolume");
        musicVCA = RuntimeManager.GetVCA("vca:/MusicVolume");
        sfxVCA = RuntimeManager.GetVCA("vca:/SFXVolume");
        uiVCA = RuntimeManager.GetVCA("vca:/UIVolume");

        // Ustawienie zakresu wartoœci suwaków (-1 do 1)
        Master.minValue = -1f;
        Master.maxValue = 1f;
        Music.minValue = -1f;
        Music.maxValue = 1f;
        SFX.minValue = -1f;
        SFX.maxValue = 1f;
        UI.minValue = -1f;
        UI.maxValue = 1f;

        // Pobranie zapisanych wartoœci lub ustawienie domyœlnych (œrodek = 0)
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0f);
        float uiVolume = PlayerPrefs.GetFloat("UIVolume", 0f);

        // Ustawienie wartoœci suwaków PRZED dodaniem listenerów, aby unikn¹æ niepotrzebnego wywo³ywania metody SetVolume
        Master.value = masterVolume;
        Music.value = musicVolume;
        SFX.value = sfxVolume;
        UI.value = uiVolume;

        // Aktualizacja g³oœnoœci w FMOD
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetUIVolume(uiVolume);

        // Dodanie listenerów PO ustawieniu wartoœci
        Master.onValueChanged.AddListener(SetMasterVolume);
        Music.onValueChanged.AddListener(SetMusicVolume);
        SFX.onValueChanged.AddListener(SetSFXVolume);
        UI.onValueChanged.AddListener(SetUIVolume);
    }

    // Konwersja wartoœci suwaka (-1 do 1) na zakres FMOD (0 do 1)
    private float ConvertSliderValue(float sliderValue)
    {
        return Mathf.Clamp01((sliderValue + 1f) / 2f);
    }

    public void SetMasterVolume(float value)
    {
        float volume = ConvertSliderValue(value);
        masterVCA.setVolume(volume);
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save(); // Zapis natychmiastowy
    }

    public void SetMusicVolume(float value)
    {
        float volume = ConvertSliderValue(value);
        musicVCA.setVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        float volume = ConvertSliderValue(value);
        sfxVCA.setVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void SetUIVolume(float value)
    {
        float volume = ConvertSliderValue(value);
        uiVCA.setVolume(volume);
        PlayerPrefs.SetFloat("UIVolume", value);
        PlayerPrefs.Save();
    }
}

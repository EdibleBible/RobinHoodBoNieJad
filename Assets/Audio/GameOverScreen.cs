using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class GameOverScreen : MonoBehaviour
{
    public EventReference gameOverEvent;

    public void PlayGameOverSound()
    {
        RuntimeManager.PlayOneShot(gameOverEvent);
    }
}


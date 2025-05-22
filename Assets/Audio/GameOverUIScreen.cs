using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class GameOverUIScreen : MonoBehaviour
{
    [SerializeField] private EventReference gameOverSound;

    private EventInstance gameOverInstance;
    private bool hasPlayed = false;

    private void OnEnable()
    {
        if (!hasPlayed)
        {
            gameOverInstance = RuntimeManager.CreateInstance(gameOverSound);
            gameOverInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            gameOverInstance.start();
            gameOverInstance.release(); // optional: only if it's a one-shot
            hasPlayed = true;
        }
    }

    private void OnDisable()
    {
        hasPlayed = false; // reset so it can play again if needed later
    }
}

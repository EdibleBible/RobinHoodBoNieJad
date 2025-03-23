using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using System.Collections;

public class EnemySoundController : MonoBehaviour
{
    public EventReference enemySound; // Przypisz event w Inspectorze
    public float minDelay = 1f;  // Minimalny czas miêdzy dŸwiêkami
    public float maxDelay = 5f;  // Maksymalny czas miêdzy dŸwiêkami

    private EventInstance enemySoundInstance;

    void Start()
    {
        StartCoroutine(PlayEnemySoundLoop());
    }

    IEnumerator PlayEnemySoundLoop()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay); // Losowy czas
            yield return new WaitForSeconds(delay);
            PlayEnemySound();
        }
    }

    void PlayEnemySound()
    {
        enemySoundInstance = RuntimeManager.CreateInstance(enemySound);
        enemySoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        enemySoundInstance.start();
        StartCoroutine(UpdateSoundPosition());
        enemySoundInstance.release();
    }

    IEnumerator UpdateSoundPosition()
    {
        while (enemySoundInstance.isValid())
        {
            enemySoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            yield return null;
        }
    }
}

using FMODUnity;
using UnityEngine;

public class EnemyIdleAudio : MonoBehaviour
{
    private StudioEventEmitter emitter;

    private void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();

        if (emitter != null)
        {
            Debug.Log("StudioEventEmitter znaleziony! Odtwarzam d�wi�k...");
            emitter.Play();
        }
        else
        {
            Debug.LogError("Brak StudioEventEmitter na przeciwniku!");
        }
    }
}

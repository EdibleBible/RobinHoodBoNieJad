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
            Debug.Log("StudioEventEmitter znaleziony! Odtwarzam dŸwiêk...");
            emitter.Play();
        }
        else
        {
            Debug.LogError("Brak StudioEventEmitter na przeciwniku!");
        }
    }
}

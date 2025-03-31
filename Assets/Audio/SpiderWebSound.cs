using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SpiderWebSound : MonoBehaviour
{
    public EventReference spiderWebEvent; // Używamy EventReference zamiast stringa

    private EventInstance spiderWebInstance;
    private bool playerInWeb = false;
    private Rigidbody playerRb;
    private bool isPlaying = false;

    private void Start()
    {
        spiderWebInstance = RuntimeManager.CreateInstance(spiderWebEvent);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInWeb = true;
            playerRb = other.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (playerInWeb && other.CompareTag("Player"))
        {
            float movement = playerRb.linearVelocity.magnitude > 0.1f ? 1f : 0f;

            spiderWebInstance.setParameterByName("Movement", movement);

            // Start eventu tylko jeśli dźwięk jeszcze nie gra
            if (movement > 0 && !isPlaying)
            {
                spiderWebInstance.start();
                isPlaying = true;
            }
            // Stop eventu, gdy gracz przestaje się poruszać
            else if (movement == 0 && isPlaying)
            {
                spiderWebInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                isPlaying = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerInWeb && other.CompareTag("Player"))
        {
            spiderWebInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            playerInWeb = false;
            isPlaying = false;
        }
    }

    private void OnDestroy()
    {
        spiderWebInstance.release();
    }
}

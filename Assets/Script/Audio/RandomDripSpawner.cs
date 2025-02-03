using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class RandomDripSpawner : MonoBehaviour
{
    public EventReference dripSound; // Przypisz event kropli w Inspectorze
    public Vector3 areaSize = new Vector3(10f, 5f, 10f); // Obszar generowania dŸwiêków
    public float minDelay = 1f;  // Minimalny czas miêdzy kroplami
    public float maxDelay = 5f;  // Maksymalny czas miêdzy kroplami

    void Start()
    {
        StartCoroutine(SpawnDripSounds());
    }

    IEnumerator SpawnDripSounds()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay); // Losowy czas
            yield return new WaitForSeconds(delay);

            // Losowe miejsce w obszarze
            Vector3 randomPosition = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(0, areaSize.y), // Wysokoœæ kropli
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            ) + transform.position;

            // Odtwarzanie dŸwiêku w danym miejscu
            PlayDripSound(randomPosition);
        }
    }

    void PlayDripSound(Vector3 position)
    {
        EventInstance dripInstance = RuntimeManager.CreateInstance(dripSound);
        dripInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        float randomVolume = Random.Range(0.5f, 1f); // Losowa g³oœnoœæ
        dripInstance.setParameterByName("Volume", randomVolume);

        dripInstance.start();
        dripInstance.release();
    }
}

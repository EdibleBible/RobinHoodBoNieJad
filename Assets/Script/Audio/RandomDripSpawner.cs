using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class RandomDripSpawner : MonoBehaviour
{
    public EventReference dripSound; // Przypisz event kropli w Inspectorze
    public Vector3 areaSize = new Vector3(10f, 5f, 10f); // Obszar generowania d�wi�k�w
    public float minDelay = 1f;  // Minimalny czas mi�dzy kroplami
    public float maxDelay = 5f;  // Maksymalny czas mi�dzy kroplami

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
                Random.Range(0, areaSize.y), // Wysoko�� kropli
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            ) + transform.position;

            // Odtwarzanie d�wi�ku w danym miejscu
            PlayDripSound(randomPosition);
        }
    }

    void PlayDripSound(Vector3 position)
    {
        EventInstance dripInstance = RuntimeManager.CreateInstance(dripSound);
        dripInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        float randomVolume = Random.Range(0.5f, 1f); // Losowa g�o�no��
        dripInstance.setParameterByName("Volume", randomVolume);

        dripInstance.start();
        dripInstance.release();
    }
}

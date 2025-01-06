using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SpawnOnNavMesh : MonoBehaviour
{
    public GameObject objectToSpawn;  // Obiekt, który ma zostać zespawnowany
    public LayerMask navMeshLayer;    // Warstwa NavMesh

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SpawnObjectOnNavMesh();
    }

    public void SpawnObjectOnNavMesh()
    {
        Vector3 spawnPosition = GetRandomPointOnNavMesh();

        if (spawnPosition != Vector3.zero)
        {
            // Spawnowanie obiektu
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

            // Dodanie NavMeshAgent
            NavMeshAgent agent = spawnedObject.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = spawnedObject.AddComponent<NavMeshAgent>();
            }

            // Ustawienie właściwości NavMeshAgent (opcjonalnie)
            agent.radius = 0.5f; // Ustawienie promienia agenta
            agent.speed = 3.5f;   // Ustawienie prędkości agenta

            // Upewnij się, że agent znajduje się na NavMesh
            PlaceAgentOnNavMesh(agent);
        }
        else
        {
            Debug.Log("Nie udało się znaleźć odpowiedniej pozycji na NavMesh.");
        }
    }

    Vector3 GetRandomPointOnNavMesh()
    {
        // Pobieranie triangulacji NavMesh (wszystkich trójkątów)
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        // Losowanie trójkąta
        int randomIndex = Random.Range(0, triangulation.indices.Length / 3) * 3;

        // Pobieranie wierzchołków trójkąta
        Vector3 vertex1 = triangulation.vertices[triangulation.indices[randomIndex]];
        Vector3 vertex2 = triangulation.vertices[triangulation.indices[randomIndex + 1]];
        Vector3 vertex3 = triangulation.vertices[triangulation.indices[randomIndex + 2]];

        // Generowanie losowego punktu w obrębie trójkąta (barycentryczne współrzędne)
        float rand1 = Random.value;
        float rand2 = Random.value;

        if (rand1 + rand2 > 1)
        {
            rand1 = 1 - rand1;
            rand2 = 1 - rand2;
        }

        float rand3 = 1 - rand1 - rand2;

        Vector3 randomPoint = rand1 * vertex1 + rand2 * vertex2 + rand3 * vertex3;

        return randomPoint;
    }

    private void PlaceAgentOnNavMesh(NavMeshAgent agent)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(agent.transform.position, out hit, 5.0f, NavMesh.AllAreas))
        {
            agent.transform.position = hit.position;
            Debug.Log("Agent został poprawnie umieszczony na NavMesh.");
        }
        else
        {
            Debug.LogError("Nie można znaleźć pobliskiego punktu NavMesh dla agenta!");
        }
    }
}
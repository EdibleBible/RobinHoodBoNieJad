using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Walk Setting")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float acceleration = 8f;   // Domyślna wartość
    [SerializeField] private float angularSpeed = 120f; // Domyślna wartość

    private Vector3 nextWalkPoint;

    [Header("Target Settings")] 
    [SerializeField] private Transform target;

    private void Awake()
    {
        // Pobranie lub przypisanie komponentu NavMeshAgent
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent nie został przypisany ani znaleziony w obiekcie!");
        }
    }

    private void Start()
    {
        // Konfiguracja prędkości i rotacji agenta
        SetUpSpeed(acceleration, angularSpeed);

        // Sprawdzenie, czy agent znajduje się na NavMesh
        if (!agent.isOnNavMesh)
        {
            TryPlaceAgentOnNavMesh();
        }
    }

    private void Update()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            Vector3 targetPosition = GetTargetPosition();
            if (targetPosition != Vector3.zero)
            {
                agent.SetDestination(targetPosition);
                Debug.Log("Agent zmienia cel na: " + targetPosition); // Debugowanie celu

                // Możesz dodać sprawdzenie, czy agent jest już blisko celu
                if (Vector3.Distance(agent.transform.position, targetPosition) < 1f)
                {
                    Debug.Log("Agent osiągnął cel.");
                }
            }
            else
            {
                Debug.LogWarning("Brak celu. Agent nie może się ruszać.");
            }
        }
        else
        {
            Debug.LogWarning("NavMeshAgent nie znajduje się na NavMesh. Próbuję ponownie umieścić go na NavMesh...");
            TryPlaceAgentOnNavMesh();
        }
    }


    /// <summary>
    /// Konfiguracja prędkości i rotacji agenta.
    /// </summary>
    private void SetUpSpeed(float acceleration, float angularSpeed)
    {
        if (agent != null)
        {
            agent.acceleration = acceleration;
            agent.angularSpeed = angularSpeed;
        }
        else
        {
            Debug.LogError("NavMeshAgent jest nullem. Nie można ustawić prędkości.");
        }
    }

    /// <summary>
    /// Pobiera pozycję celu.
    /// </summary>
    private Vector3 GetTargetPosition()
    {
        if (target != null)
        {
            return target.position;
        }
        else
        {
            Debug.LogWarning("Brak przypisanego celu dla agenta!");
            return Vector3.zero;
        }
    }

    /// <summary>
    /// Próbuje umieścić agenta na NavMesh w pobliżu obecnej pozycji.
    /// </summary>
    private void TryPlaceAgentOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log("Agent został poprawnie umieszczony na NavMesh.");
        }
        else
        {
            Debug.LogError("Nie można znaleźć pobliskiego punktu NavMesh dla agenta!");
        }
    }
}

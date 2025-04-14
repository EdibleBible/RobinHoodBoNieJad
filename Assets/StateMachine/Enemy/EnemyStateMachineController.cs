using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using FMODUnity;

public class EnemyStateMachineController : StateManager<E_EnemyState>
{
    private EnemyPatrollingState _enemyPatrollingState;
    private EnemyChasingState _enemyChasingState;
    private EnemyAlarmedState _enemyAlarmedState;

    [SerializeField]
    private float findingPointDistance;
    public float maxNavMeshCheckDistance = 5f;

    private EnemyMovement enemyMovement;
    private FieldOfView fov;

    [Header("patrolling state settings")]
    [SerializeField]
    private EnemyMovementStats patrollingMovementStats;
    [SerializeField]
    private EnemyFovStats patrollingFovStats;

    [Header("chasing state settings")]
    [SerializeField]
    private EnemyMovementStats chasingMovementStats;
    [SerializeField]
    private EnemyFovStats chasingFovStats;

    [Header("alarmed state settings")]
    [SerializeField]
    private EnemyMovementStats alarmedMovementStats;
    [SerializeField]
    private EnemyFovStats alarmedFovStats;
    public EnemyAlarmedStats alarmedStateStats = new EnemyAlarmedStats();

    [Header("Audio")]
    [SerializeField] private EventReference noticePlayerEvent;
    private bool hasPlayedNoticeSound = false;

    [SerializeField] private EventReference footstepsLoopEvent;
    private FMOD.Studio.EventInstance footstepsInstance;
    private bool isFootstepsPlaying = false;



    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        fov = GetComponent<FieldOfView>();
    }

    public override void Start()
    {
        EnsureOnNavMesh(); // Sprawdzenie i ustawienie postaci na NavMeshu

        _enemyPatrollingState =
            new EnemyPatrollingState(enemyMovement, patrollingMovementStats, fov, patrollingFovStats,
                alarmedStateStats);
        _enemyChasingState =
            new EnemyChasingState(enemyMovement, chasingMovementStats, fov, chasingFovStats, alarmedStateStats);
        _enemyAlarmedState =
            new EnemyAlarmedState(enemyMovement, alarmedMovementStats, fov, alarmedFovStats, alarmedStateStats);

        state.Add(E_EnemyState.Alarmed, _enemyAlarmedState);
        state.Add(E_EnemyState.Patrol, _enemyPatrollingState);
        state.Add(E_EnemyState.Chase, _enemyChasingState);

        currentState = _enemyPatrollingState;
        base.Start();
    }

    public override void Update()
    {
        E_EnemyState nextStateKey = currentState.GetNextState();
        if (!isTransitioningState && nextStateKey.Equals(currentState.stateKey))
        {
            currentState.UpdateState();
        }
        else if (!isTransitioningState)
        {
            // Sprawdzenie, czy to pierwszy raz zauważono gracza
            if (!hasPlayedNoticeSound && (nextStateKey == E_EnemyState.Alarmed || nextStateKey == E_EnemyState.Chase))
            {
                RuntimeManager.PlayOneShot(noticePlayerEvent, transform.position);
                hasPlayedNoticeSound = true;
            }

            if (nextStateKey == E_EnemyState.Patrol)
            {
                hasPlayedNoticeSound = false;
            }

            TransitionToState(nextStateKey);
        }

        if (nextStateKey == E_EnemyState.Chase)
        {
            Debug.Log("Przechodzę do stanu CHASE – uruchamiam kroki");
            StartFootsteps();
        }
        else
        {
            StopFootsteps(); // zatrzymuje, jeśli nie goni
        }


        if (Input.GetMouseButtonDown(0)) // Sprawdza, czy kliknięto lewym przyciskiem myszy
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Tworzy promień z pozycji kamery
            if (Physics.Raycast(ray, out RaycastHit hit)) // Sprawdza, czy promień trafił w coś
            {
                var HitPoint = hit.point; // Zapisuje pozycję trafienia
                alarmedStateStats.IsAlarmed = true;
                alarmedStateStats.AlarmedPosition = HitPoint;
            }
        }
    }

    private void EnsureOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, maxNavMeshCheckDistance, NavMesh.AllAreas))
        {
            transform.position = hit.position; // Ustawienie pozycji na NavMeshu
        }
        else
        {
            Debug.LogError("Enemy could not find NavMesh near its spawn position!", this);
        }
    }

    private void StartFootsteps()
    {
        if (!isFootstepsPlaying)
        {
            footstepsInstance = RuntimeManager.CreateInstance(footstepsLoopEvent);
            RuntimeManager.AttachInstanceToGameObject(footstepsInstance, transform, GetComponent<Rigidbody>());
            footstepsInstance.start();
            isFootstepsPlaying = true;
        }
    }

    private void StopFootsteps()
    {
        if (isFootstepsPlaying)
        {
            footstepsInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            footstepsInstance.release();
            isFootstepsPlaying = false;
        }
    }

}

/*[Serializable]
public class EnemyMovementStats
{
    public float MaxSpeed;
    public float Acceleration;
    public float AngularSpeed;
    public float DelayBettwenDistanceCheck;
    public float StoppingDistance;
    public float ChangeDestinationPointDistance;
}

[Serializable]
public class EnemyPatrollingStats
{
    public int MaxPatrolPoints;
    [HideInInspector] public List<Vector3> PatrolPoints = new List<Vector3>(); // Lista pozycji
    public List<Transform> PatrolTransforms = new List<Transform>(); // Lista transformów
    public float MinDistanceBetweenPatrolPoints;
    public float MaxDistanceBetweenPatrolPoints;

    public bool RandomPointPatroll;
    public bool UseCreatedPath;
    public bool LoopPatrolPoints;
    private bool forward;
    private bool isChanged;

    private Vector3 currentPatrolPoint;
    private int currentPatrolPointIndex;

    public void CreatePatrolPoint(Vector3 startPoint)
    {
        PatrolPoints.Clear();

        // Ustaw początkowy punkt na pozycji startPoint
        Vector3 currentPoint = startPoint;
        PatrolPoints.Add(currentPoint);

        for (int i = 1; i < MaxPatrolPoints; i++)
        {
            Vector3 nextPoint;

            if (FindRandomPoint(currentPoint, MinDistanceBetweenPatrolPoints, MaxDistanceBetweenPatrolPoints,
                    out nextPoint))
            {
                PatrolPoints.Add(nextPoint);
                currentPoint = nextPoint; // Ustaw nowy punkt jako centrum dla kolejnego
            }
            else
            {
                Debug.LogWarning($"Nie udało się znaleźć punktu dla indeksu {i}");
                break;
            }
        }
    }

    public void UpdatePatrolPoints()
    {
        PatrolPoints.Clear(); // Czyścimy starą listę

        foreach (Transform patrolTransform in PatrolTransforms)
        {
            if (patrolTransform != null)
            {
                PatrolPoints.Add(patrolTransform.position); // Dodajemy aktualną pozycję transformu
            }
        }
    }

    bool FindRandomPoint(Vector3 center, float minDist, float maxDist, out Vector3 result)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxDist;
            randomDirection.y = 0;
            randomDirection += center;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, maxDist, NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(center, hit.position);
                if (distance >= minDist && distance <= maxDist)
                {
                    result = hit.position;
                    return true;
                }
            }
        }

        result = Vector3.zero;
        return false;
    }

    public void ChangeNextPatrolPoint()
    {
        /*if(isChanged)
            return;

        int currIndex = PatrolPoints.IndexOf(currentPatrolPoint);
        if (LoopPatrolPoints)
        {
            if (currIndex++ >= PatrolPoints.Count)
            {
                currIndex = 0;
            }
        }
        else
        {
            if (forward)
            {
                if (currIndex++>= PatrolPoints.Count)
                {
                    currIndex = PatrolPoints.Count - 2;
                    forward = false;
                }
            }
            else
            {
                if (currIndex-- <= -1)
                {
                    currIndex = 0;
                    forward = true;
                }
            }
        }
        Debug.Log("currPoint: " + currIndex);
        isChanged = true;
        currentPatrolPoint = PatrolPoints[currIndex];#1#
    }

    public Vector3 GetNextPatrollingPoint()
    {
        if (LoopPatrolPoints)
        {
            // Zwiększ indeks przed sprawdzeniem warunku
            currentPatrolPointIndex++;
            if (currentPatrolPointIndex >= PatrolPoints.Count)
            {
                currentPatrolPointIndex = 0;
            }
        }
        else
        {
            if (forward)
            {
                currentPatrolPointIndex++;
                if (currentPatrolPointIndex >= PatrolPoints.Count)
                {
                    currentPatrolPointIndex = PatrolPoints.Count - 2;
                    forward = false;
                }
            }
            else
            {
                currentPatrolPointIndex--;
                if (currentPatrolPointIndex < 0)
                {
                    currentPatrolPointIndex = 1;
                    forward = true;
                }
            }
        }

        currentPatrolPoint = PatrolPoints[currentPatrolPointIndex];
        return currentPatrolPoint;
    }

    public Vector3 GetCurrentPatrollingPoint()
    {
        return currentPatrolPoint;
    }

    public Vector3 GetNearestPoint(Vector3 position)
    {
        float distance = Vector3.Distance(position, currentPatrolPoint);
        Vector3 closestPoint = currentPatrolPoint;

        foreach (var points in PatrolPoints)
        {
            float newDistance = Vector3.Distance(points, position);
            if (distance > newDistance)
            {
                distance = newDistance;
                closestPoint = points;
            }
        }

        return closestPoint;
    }
}
[Serializable]
public class EnemyAlarmedStats
{
    public int SmallPatrolCount = 3; // Ile razy przeciwnik patroluje po dotarciu
    public float SmallPatrolRadius = 2.0f; // Promień małego patrolowania
    public Vector3 AlarmedPlacePostion;
    public bool IsAlarmed;
}*/
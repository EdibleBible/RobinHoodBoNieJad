using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyStateMachineController : StateManager<E_EnemyState>
{
    private EnemyPatrollingState _enemyPatrollingState;
    private EnemyChasingState _enemyChasingState;

    [SerializeField] private float findingPointDistance;

    [Header("Idle State")] [SerializeField]
    private EnemyMovementStats idleStats;

    [Header("Patrolling State")] [SerializeField]
    private EnemyPatrollingStats patrollingStats;

    [SerializeField] private EnemyMovementStats patrollingMoveStats;
    [SerializeField] private EnemyFovStats fovPatrollingState;

    [Header("Chase State")] [SerializeField]
    private EnemyMovementStats chasingStats;

    [SerializeField] private EnemyFovStats fovChaseState;


    private EnemyMovement enemyMovement;
    private FieldOfView fov;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        fov = GetComponent<FieldOfView>();
    }

    public override void Start()
    {
        _enemyPatrollingState = new EnemyPatrollingState(fov, fovPatrollingState, enemyMovement, patrollingMoveStats,
            patrollingStats, findingPointDistance);
        _enemyChasingState = new EnemyChasingState(fov, fovPatrollingState, enemyMovement, chasingStats);


        state.Add(E_EnemyState.Patrol, _enemyPatrollingState);
        state.Add(E_EnemyState.Chase, _enemyChasingState);

        currentState = _enemyPatrollingState;
        base.Start();
    }

    private void OnDrawGizmos()
    {
        if (currentState == _enemyPatrollingState && !patrollingStats.RandomPointPatroll)
        {
            foreach (var points in patrollingStats.PatrolPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(points, 0.4f);
            }
        }
    }
}

[Serializable]
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
public class EnemyFovStats
{
    public float ViewRadius;
    public float ViewAngle;
    public float FindingDelay;

    public LayerMask TargetLayer;
    public LayerMask ObstacleLayer;
}

[Serializable]
public class EnemyPatrollingStats
{
    public int MaxPatrolPoints;
    public List<Vector3> PatrolPoints;
    public float MinDistanceBetweenPatrolPoints;
    public float MaxDistanceBetweenPatrolPoints;

    public bool RandomPointPatroll;
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
        currentPatrolPoint = PatrolPoints[currIndex];*/
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
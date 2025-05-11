using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent Agent;
    public EnemyAnimationController AnimationController;
    [HideInInspector] public Queue<Vector3> destinations = new();
    private Vector3 nextDestination;
    public event Action OnDestinationReached;
    public event Action OnStopLookingAround;

    private Coroutine lookingAroundCoroutine;
    private Coroutine checkingDistanceCoroutine;

    [Header("Settings")] public bool RandomPatroll = true;
    public List<Transform> Waypoints;
    public int CurrentWaypoint = 0;

    [Header("Door Detection")] public LayerMask IntractableLayer;
    public GameObject Model;
    public Vector3 HiddenScale;
    public Vector3 NormalScale;
    public float ScaleDuration;

    private void Start()
    {
        EnsureAgentOnNavMesh();
    }

    public void PathFind()
    {
        ClearPath();
        if (RandomPatroll)
        {
            var randomPosition = FindRandomPointAroundPoint(transform.position, 10f);
            destinations.Enqueue(randomPosition);
            destinations.Enqueue(FindRandomPointAroundPoint(randomPosition, 10f));
        }
        else
        {
            if (Waypoints.Count < 2)
            {
                Debug.LogWarning("You need 2 or more waypoints");
                return;
            }

            if (CurrentWaypoint >= Waypoints.Count)
                CurrentWaypoint = 0;

            destinations.Enqueue(Waypoints[CurrentWaypoint].position);
            CurrentWaypoint++;
            if (CurrentWaypoint >= Waypoints.Count)
                CurrentWaypoint = 0;
            destinations.Enqueue(Waypoints[CurrentWaypoint].position);
            CurrentWaypoint++;
        }
    }

    public int GetDestinationCount()
    {
        return destinations.Count;
    }

    public void SetUpParameters(EnemyMovementStats stats)
    {
        Agent.speed = stats.MaxSpeed;
        Agent.angularSpeed = stats.AngularSpeed;
        Agent.acceleration = stats.Acceleration;
        Agent.stoppingDistance = stats.StoppingDistance;
    }

    public Vector3 FindRandomPointAroundPoint(Vector3 point, float radius)
    {
        if (point == Vector3.zero)
        {
            point = transform.position;
        }

        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += point;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    public void GoToNextPoint(bool withAddNewWaypoint = true)
    {
        nextDestination = destinations.Dequeue();
        SetEnemyDestination(nextDestination);

        if (!withAddNewWaypoint)
            return;

        if (RandomPatroll)
        {
            Vector3 firstDestination = destinations.Peek();
            destinations.Enqueue(FindRandomPointAroundPoint(firstDestination, 10f));
        }
        else
        {
            if (CurrentWaypoint > Waypoints.Count - 1)
            {
                CurrentWaypoint = 0;
                destinations.Enqueue(Waypoints[CurrentWaypoint].position);
            }
            else
            {
                destinations.Enqueue(Waypoints[CurrentWaypoint].position);
                CurrentWaypoint++;
            }
        }
    }

    public void AddWaypoint(Vector3 position)
    {
        destinations.Enqueue(position);
    }

    public void SetEnemyDestination(Vector3 destination, bool _chekingDistance = true)
    {
        if (_chekingDistance)
        {
            StopCheckingDistance();
        }

        Agent.SetDestination(destination);

        if (_chekingDistance)
        {
            StartCheckingDistance(0.5f);
        }
        
        if (stuckCheckCoroutine != null)
            StopCoroutine(stuckCheckCoroutine);

        stuckCheckCoroutine = StartCoroutine(MonitorStuckState());
    }

    public void StartLookingAround(float waitingTime)
    {
        AnimationController.SetLookAround(true);
        lookingAroundCoroutine = StartCoroutine(LookingAroundCoroutine(waitingTime));
    }

    public void StopLookingAround()
    {
        if (lookingAroundCoroutine != null)
        {
            StopCoroutine(lookingAroundCoroutine);
        }

        AnimationController.SetLookAround(false);
        OnStopLookingAround?.Invoke();
    }

    public IEnumerator LookingAroundCoroutine(float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            yield return new WaitForSeconds(0.5f); // Aktualizuj co sekundę
            elapsedTime += 1f;
        }

        StopLookingAround();
    }

    public void StartCheckingDistance(float waitingTime)
    {
        checkingDistanceCoroutine = StartCoroutine(CheckDistanceCoroutine(waitingTime));
    }

    public void StopCheckingDistance()
    {
        if (checkingDistanceCoroutine != null)
        {
            StopCoroutine(checkingDistanceCoroutine);
        }
    }

    public IEnumerator CheckDistanceCoroutine(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            if (!Agent.isActiveAndEnabled || !Agent.isOnNavMesh)
            {
                yield break; // kończymy coroutine, bo agent nie działa poprawnie
            }

            var distanceToDestination = Agent.remainingDistance;
            if (distanceToDestination <= Agent.stoppingDistance)
            {
                OnDestinationReached?.Invoke();
                yield break;
            }
        }
    }

    private void EnsureAgentOnNavMesh()
    {
        if (!Agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                Debug.Log("Agent moved to nearest NavMesh point.");
            }
            else
            {
                Debug.LogWarning("No NavMesh nearby! Agent cannot move.");
            }
        }
    }


    public void UpdateMoveAnimation(float _speed)
    {
        AnimationController.UpdateWalkParameters(_speed);
    }

    public void ClearPath()
    {
        destinations = new Queue<Vector3>();
    }

    public bool CheckObjectIsOnAgentPath(GameObject objectToCheck)
    {
        if (Agent == null || Agent.path == null || Agent.path.corners.Length < 2)
            return false;

        for (int i = 0; i < Agent.path.corners.Length - 1; i++)
        {
            Vector3 from = Agent.path.corners[i];
            Vector3 to = Agent.path.corners[i + 1];
            Vector3 direction = (to - from).normalized;
            float distance = Vector3.Distance(from, to);
            RaycastHit[] hits = Physics.RaycastAll(from, direction, distance, IntractableLayer);
            foreach (var hit in hits)
            {
                if (ComponentUtils.TryGetComponentInParent(hit.collider.gameObject, out DoorController door))
                {
                    if (door.gameObject == objectToCheck)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool IsObjectOnPathAndNearby(float raycastDistance)
    {
        if (Agent.path == null || Agent.path.corners.Length < 2)
        {
            return false;
        }

        Vector3
            rayOrigin = Agent.transform.position +
                        Vector3.up * 0.5f;
        Vector3 rayDirection = Agent.transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, raycastDistance, IntractableLayer))
        {
            if (ComponentUtils.TryGetComponentInParent(hit.collider.gameObject, out DoorController door))
            {
                if (CheckObjectIsOnAgentPath(door.gameObject))
                {
                    Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.green);
                    return true;
                }
                else
                {
                    Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.yellow);
                    return false;
                }
            }
            else
            {
                Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.blue);
                return false;
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.red);
            return false;
        }
    }


    private float DistancePointToLineSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        Vector3 ap = point - a;

        float magnitudeAB = ab.sqrMagnitude;
        if (magnitudeAB == 0f) return Vector3.Distance(point, a);

        float dot = Vector3.Dot(ap, ab);
        float t = Mathf.Clamp01(dot / magnitudeAB);
        Vector3 closestPoint = a + t * ab;

        return Vector3.Distance(point, closestPoint);
    }

    private Coroutine stuckCheckCoroutine;
    private Vector3 lastPosition;
    private float stuckTimer;

    private IEnumerator MonitorStuckState(float checkInterval = 1f, float stuckTimeThreshold = 3f)
    {
        stuckTimer = 0f;
        lastPosition = transform.position;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            float movedDistance = Vector3.Distance(transform.position, lastPosition);

            if (movedDistance < 0.1f) // prawie się nie ruszył
            {
                stuckTimer += checkInterval;

                if (stuckTimer >= stuckTimeThreshold)
                {
                    Debug.LogWarning("Enemy seems stuck. Resolving...");

                    TryResolveStuck();
                    yield break;
                }
            }
            else
            {
                stuckTimer = 0f;
            }

            lastPosition = transform.position;
        }
    }

    private void TryResolveStuck()
    {
        // Próbujemy znaleźć nowy punkt w pobliżu
        if (NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * 5f, out NavMeshHit hit, 5f,
                NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log("Enemy teleported to nearby NavMesh point.");
            PathFind();
            GoToNextPoint();
        }
        else
        {
            Debug.LogWarning("No NavMesh nearby to resolve stuck.");
        }
    }


    private void Update()
    {
        bool anyDoorRelevant;


        if (IsObjectOnPathAndNearby(2))
        {
            anyDoorRelevant = true;
        }
        else
        {
            anyDoorRelevant = false;
        }


        if (Model != null)
        {
            Vector3 targetScale = anyDoorRelevant ? HiddenScale : NormalScale;
            Model.transform.DOScale(targetScale, ScaleDuration).SetEase(Ease.InOutSine);
        }
    }
}

[Serializable]
public class EnemyMovementStats
{
    public float MaxSpeed;
    public float Acceleration;
    public float AngularSpeed;
    public float StoppingDistance;
    public float LookingAroundTime;
}

public static class ComponentUtils
{
    public static bool TryGetComponentInParent<T>(this GameObject obj, out T component) where T : Component
    {
        component = obj.GetComponentInParent<T>();
        return component != null;
    }
}
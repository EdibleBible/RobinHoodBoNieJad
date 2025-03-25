using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent Agent;
    public EnemyAnimationController AnimationController;
    private Queue<Vector3> destinations = new();
    private Vector3 nextDestination;
    public event Action OnDestinationReached;
    public event Action OnStopLookingAround;

    private Coroutine lookingAroundCoroutine;
    private Coroutine checkingDistanceCoroutine;

    [Header("Settings")]
    public bool RandomPatroll = true;
    public List<Transform> Waypoints;
    public int CurrentWaypoint = 0;
    
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

            destinations.Enqueue(Waypoints[CurrentWaypoint].position);
            CurrentWaypoint++;
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

        if(!withAddNewWaypoint)
            return;
        
        if (RandomPatroll)
        {
            Vector3 firstDestination = destinations.Peek();
            destinations.Enqueue(FindRandomPointAroundPoint(firstDestination, 10f));
        }
        else
        {
            if (CurrentWaypoint >= Waypoints.Count - 1)
            {
                CurrentWaypoint = 0;
                destinations.Enqueue(Waypoints[CurrentWaypoint].position);
            }
            else
            {
                CurrentWaypoint++;
                destinations.Enqueue(Waypoints[CurrentWaypoint].position);
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
            StopCheckingDistance();

        Agent.SetDestination(destination);

        if (_chekingDistance) 
            StartCheckingDistance(0.5f);
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
            var distanceToDestination = Agent.remainingDistance;
            if (distanceToDestination <= Agent.stoppingDistance)
            {
                OnDestinationReached?.Invoke();
                yield break;
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
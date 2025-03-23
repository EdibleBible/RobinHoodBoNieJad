using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Walk Setting")]
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField] private Vector3 currWalkPosition;
    private EnemyAnimationController animatorController;
    private float distanceToDestination;
    private Coroutine checkDistanceCoroutine;
    private Coroutine waitCoroutine;
    private bool isCoroutineRunning = false;

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

        if (animatorController == null)
        {
            animatorController = GetComponent<EnemyAnimationController>();
        }
    }

    public void SetUpSpeed(float _speed, float _accelerationTime, float _angularSpeed, float _stoppingDistance)
    {
        agent.speed = _speed;
        agent.acceleration = _accelerationTime;
        agent.angularSpeed = _angularSpeed;
        agent.stoppingDistance = _stoppingDistance;
    }

    public Vector3 FindRandomPointOnNavMesh(float _FindingPointDistance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * _FindingPointDistance;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, _FindingPointDistance, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    public bool IsPointOnNavMesh(Vector3 _point, float _range = 1.0f)
    {
        NavMeshHit hit;
        // Sample the nearest point on the NavMesh
        if (NavMesh.SamplePosition(_point, out hit, _range, NavMesh.AllAreas))
        {
            // Check if the sampled point is within the specified range
            float distance = Vector3.Distance(_point, hit.position);
            return distance <= _range;
        }

        return false; // Point is not on the NavMesh
    }

    public void RunCoroutine(Vector3 nextPoint, float _waitTime)
    {
        if (isCoroutineRunning)
            return; // Ignorujemy kolejne wywołania, jeśli korutyna już działa

        waitCoroutine = StartCoroutine(WaitToFindingNewPoint(nextPoint, _waitTime));
    }

    public void SetupLookAround(bool isLookingAround)
    {
        animatorController.SetLookAround(isLookingAround); // Start animacji "rozglądania się"
        isCoroutineRunning = false;
    }

    private IEnumerator WaitToFindingNewPoint(Vector3 nextPoint, float _waitTime)
    {
        isCoroutineRunning = true;
        animatorController.SetLookAround(true);

        float elapsedTime = 0f;
        while (elapsedTime < _waitTime)
        {
            Debug.Log($"Pozostały czas do zmiany punktu: {_waitTime - elapsedTime:F2} s");
            yield return new WaitForSeconds(1f); // Aktualizuj co sekundę
            elapsedTime += 1f;
        }

        SetDestination(nextPoint);
        ResetDestination();

        isCoroutineRunning = false;
    }


    public void SetDestination(Vector3 _position)
    {
        currWalkPosition = _position;
        agent.SetDestination(_position);
    }

    public float ResetDestination()
    {
        distanceToDestination = agent.remainingDistance;
        return distanceToDestination;
    }

    public IEnumerator CheckDistance(float _delayInSec)
    {
        while (true)
        {
            yield return new WaitForSeconds(_delayInSec);
            distanceToDestination = agent.remainingDistance;
            Debug.LogWarning("Check distance. Value is: " + distanceToDestination);
        }
    }

    public void UpdateMoveAnimation(float _speed)
    {
        animatorController.UpdateWalkParameters(_speed);
    }

    public void SetDistance()
    {
        distanceToDestination = agent.remainingDistance;
    }

    public float GetDistanceToDestination()
    {
        return distanceToDestination;
    }

    public void ResetCheckingDistance(float _delayInSec)
    {
        if (checkDistanceCoroutine != null)
            StopCoroutine(checkDistanceCoroutine);
        checkDistanceCoroutine = null;
        checkDistanceCoroutine = StartCoroutine(CheckDistance(_delayInSec));
    }

    public void StopCheckingDistance()
    {
        if (checkDistanceCoroutine != null)
            StopCoroutine(checkDistanceCoroutine);
    }

    public float GetNormalizedSpeed()
    {
        if (agent == null || agent.speed <= 0) return 0f;

        float currentSpeed = agent.velocity.magnitude;
        return Mathf.Clamp01(currentSpeed / agent.speed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(currWalkPosition, 0.1f);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
            Destroy(other.gameObject);
    }

    public void StopLookingCoroutine()
    {
        StopCoroutine(waitCoroutine);
    }
}
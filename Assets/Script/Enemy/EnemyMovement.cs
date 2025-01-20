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
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Vector3 currWalkPosition;
    private float distanceToDestination;
    private Coroutine checkDistanceCoroutine;
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
    public void SetUpSpeed(float _speed, float _accelerationTime, float _decelerationTime, float _angularSpeed, float _stoppingDistance)
    {
        agent.speed = _speed;
        agent.acceleration = _accelerationTime;
        agent.stoppingDistance = _decelerationTime;
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
        if(checkDistanceCoroutine != null)
            StopCoroutine(checkDistanceCoroutine);
        checkDistanceCoroutine = null;
        checkDistanceCoroutine = StartCoroutine(CheckDistance(_delayInSec));
    }

    public void StopCheckingDistance()
    {
        if(checkDistanceCoroutine != null)
            StopCoroutine(checkDistanceCoroutine);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(currWalkPosition,0.1f);
    }

    public void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
            Destroy(other.gameObject);
    }
}
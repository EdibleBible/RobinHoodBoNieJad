using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FieldOfView : MonoBehaviour
{
    private float viewRadius;
    private float viewAngle;

    [SerializeField] private Transform fieldOfViewCaster;
    private LayerMask obstacleMask;
    private LayerMask targetMask;
    private List<Transform> visibleTargets = new List<Transform>();
    private Coroutine startedCoroutine;

    [Header("Gizmos Settings")]
    [SerializeField] private bool showFOVGizmos = true; // Czy rysować Gizmo dla FOV

    public List<Transform> GetVisibleTargets()
    {
        return visibleTargets;
    }

    public void SetUpStats(EnemyFovStats _parameters)
    {
        viewRadius = _parameters.ViewRadius;
        viewAngle = _parameters.ViewAngle;

        obstacleMask = _parameters.ObstacleLayer;
        targetMask = _parameters.TargetLayer;
    }
    
    public void StartFindingTargets(float _delay)
    {
        startedCoroutine = StartCoroutine(FindTargetWithDelay(_delay));
    }

    public void StopFindingTargets()
    {
        if(startedCoroutine != null)
            StopCoroutine(startedCoroutine);
        startedCoroutine = null;
    }

    public void ResetFindingTargets(float _delay)
    {
        StopFindingTargets();
        StartFindingTargets(_delay);
    }
    
    private IEnumerator FindTargetWithDelay(float _delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(_delay);
            FindVisibleTargets();
        }
    }
    
    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(fieldOfViewCaster.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - fieldOfViewCaster.position).normalized;

            if (Vector3.Angle(fieldOfViewCaster.forward, dirToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(fieldOfViewCaster.position, target.position);
                if (!Physics.Raycast(fieldOfViewCaster.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public float GetViewAngle()
    {
        return viewAngle;
    }

    public float GetViewRadius()
    {
        return viewRadius;
    }
    
    public Vector3 DirFromAngle(float _angleDegrees, bool _angleIsGlobal)
    {
        if (!_angleIsGlobal)
        {
            _angleDegrees += fieldOfViewCaster.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(_angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(_angleDegrees * Mathf.Deg2Rad));
    }

    public (bool isInRange,GameObject target) IsTargetInRange()
    {
        if (visibleTargets.Count <= 0)
        {
            return (false, null);
        }
        else
        {
            return (true, visibleTargets[0].gameObject);
        }
    }

    public Vector3 GetTargePosition(int targetIndex)
    {
        return visibleTargets[targetIndex].position;
    }

    private void OnDrawGizmos()
    {
        if (!showFOVGizmos || fieldOfViewCaster == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(fieldOfViewCaster.position, viewRadius); // Rysowanie okręgu zasięgu widzenia

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2, false) * viewRadius;
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2, false) * viewRadius;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(fieldOfViewCaster.position, fieldOfViewCaster.position + leftBoundary);
        Gizmos.DrawLine(fieldOfViewCaster.position, fieldOfViewCaster.position + rightBoundary);

        Gizmos.color = Color.red;
        foreach (Transform target in visibleTargets)
        {
            Gizmos.DrawSphere(target.position, 0.2f); // Oznaczenie widocznych celów
        }
    }
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

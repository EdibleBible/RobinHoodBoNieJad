using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpecialSenseController : MonoBehaviour
{
    [SerializeField] private float maxRadius = 10f;
    [SerializeField] private float expandSpeed = 10f;
    [SerializeField] private LayerMask detectionLayers;
    [SerializeField] private LayerMask OccludedInteractionLayer;

    [SerializeField] private float highlightDuration = 2f;
    [SerializeField] private KeyCode highlightKey = KeyCode.E;

    [Header("Line Renderer Settings")]
    [SerializeField] private LineRenderer lineRendererPrefab;
    [SerializeField] private float lineSpeed = 20f;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem particlePrefab;

    private Dictionary<GameObject, int> activeHighlights = new();
    private Coroutine pulseCoroutine;
    private float currentRadius = 0f;
    private Vector3 pulsePosition;

    void Update()
    {
        if (Input.GetKeyDown(highlightKey))
        {
            pulsePosition = transform.position;
            if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
            pulseCoroutine = StartCoroutine(PulseDetection());
        }
    }

    private IEnumerator PulseDetection()
    {
        currentRadius = 0f;
        ClearHighlights();

        while (currentRadius < maxRadius)
        {
            currentRadius += expandSpeed * Time.deltaTime;
            Collider[] hits = Physics.OverlapSphere(pulsePosition, currentRadius, detectionLayers);

            foreach (var hit in hits)
            {
                GameObject obj = hit.gameObject;
                Transform root = obj.transform.root;
                GameObject rootObj = root.gameObject;

                if (activeHighlights.ContainsKey(rootObj))
                    continue;

                bool alreadyContained = false;
                foreach (Transform child in root)
                {
                    if (activeHighlights.ContainsKey(child.gameObject))
                    {
                        alreadyContained = true;
                        break;
                    }
                }

                if (alreadyContained)
                    continue;

                Debug.Log(rootObj.name);
                activeHighlights.Add(rootObj, rootObj.layer);
                StartCoroutine(AnimateLineToTarget(rootObj));
            }

            yield return null;
        }

        yield return new WaitForSeconds(highlightDuration);
        yield return StartCoroutine(ClearHighlightsStepByStep());
    }

    private IEnumerator AnimateLineToTarget(GameObject target)
    {
        LineRenderer line = null;
        ParticleSystem particles = null;

        if (lineRendererPrefab != null)
        {
            line = Instantiate(lineRendererPrefab, transform.position, Quaternion.identity);
            line.positionCount = 2;
            line.SetPosition(0, transform.position);
        }

        if (particlePrefab != null)
        {
            particles = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            particles.Play();
        }

        float duration = Vector3.Distance(transform.position, target.transform.position) / lineSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 currentPos = Vector3.Lerp(transform.position, target.transform.position, t);

            if (line != null)
                line.SetPosition(1, currentPos);

            if (particles != null)
                particles.transform.position = currentPos;

            yield return null;
        }

        if (line != null)
            line.SetPosition(1, target.transform.position);

        if (particles != null)
        {
            particles.transform.position = target.transform.position;
            particles.Stop();
            Destroy(particles.gameObject, 1f);
        }

        ToggleHighlight(target, true);

        if (line != null)
            Destroy(line.gameObject, 0.5f);
    }

    private IEnumerator ClearHighlightsStepByStep()
    {
        foreach (var obj in activeHighlights)
        {
            ToggleHighlight(obj.Key, false);
            yield return new WaitForSeconds(0.05f);
        }

        activeHighlights.Clear();
    }

    private void ClearHighlights()
    {
        foreach (var obj in activeHighlights)
            ToggleHighlight(obj.Key, false);

        activeHighlights.Clear();
    }

    private int LayerMaskToLayer(LayerMask mask)
    {
        int value = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((value & (1 << i)) != 0)
                return i;
        }
        Debug.LogWarning("LayerMask contains multiple layers, using the first found.");
        return 0;
    }

    private void ToggleHighlight(GameObject obj, bool on)
    {
        if (on)
        {
            SetLayerRecursively(obj, LayerMaskToLayer(OccludedInteractionLayer), detectionLayers);
        }
        else
        {
            if (activeHighlights.TryGetValue(obj, out int originalLayer))
            {
                SetLayerRecursively(obj, originalLayer, OccludedInteractionLayer);
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer, LayerMask maskToCheck)
    {
        if (obj == null) return;

        if (IsInLayerMask(obj, maskToCheck))
        {
            obj.layer = newLayer;
        }

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer, maskToCheck);
        }
    }

    private bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.DrawSphere(transform.position, currentRadius);
    }
}

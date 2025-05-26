using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.ScriptableObjects;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

/// <summary>
/// SpecialSenseController — skan otoczenia, highlighty, particles + audio FMOD
/// Dźwięk startuje przy aktywacji, zapętla się i wycisza dokładnie po zakończeniu efektu.
/// </summary>
public class SpecialSenseController : MonoBehaviour
{
    /* ───────────────────────────────── Inspector ─────────────────────────────── */
    [Header("Pulse Settings")]
    [SerializeField] private float baseMaxRadius = 5f;
    [SerializeField] private float expandSpeed = 10f;
    [SerializeField] private LayerMask detectionLayers;
    [SerializeField] private LayerMask occludedInteractionLayer;
    [SerializeField] private float highlightDuration = 2f;

    [Header("Line Renderer Settings")]
    [SerializeField] private LineRenderer lineRendererPrefab;
    [SerializeField] private float lineSpeed = 20f;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem particlePrefab;

    [Header("Gameplay Balancing")]
    [SerializeField] private int baseMaxLinesPerPulse = 3;
    [SerializeField] private int baseSpecialSenseUseCount = 2;

    [Header("Audio (FMOD)")]
    [SerializeField] private EventReference specialSenseEvent; // event:/Abilities/SpecialSense

    /* ──────────────────────────────── Private state ──────────────────────────── */
    private readonly Dictionary<GameObject, int> activeHighlights = new();

    [SerializeField] private SOPlayerStatsController statsController;

    private Coroutine pulseCoroutine;
    private EventInstance senseInstance;

    private float maxRadius;
    private float currentRadius;
    private int maxLinesPerPulse;
    private int specialSenseUseCount;
    private bool isCooldown;
    private Vector3 pulsePosition;

    /* ───────────────────────────── Unity lifecycle ───────────────────────────── */
    private void Start() => SetupSpecialSense();

    private void OnDestroy()
    {
        if (senseInstance.isValid()) senseInstance.release();
    }

    /* ───────────────────────────── Public API ────────────────────────────────── */
    public void SetupSpecialSense()
    {
        maxLinesPerPulse = Mathf.FloorToInt(baseMaxLinesPerPulse +
                            statsController.GetSOPlayerStats(E_ModifiersType.SpecialSenseGhostAmount).Additive) *
                           Mathf.FloorToInt(statsController.GetSOPlayerStats(E_ModifiersType.SpecialSenseGhostAmount).Multiplicative);

        maxRadius = (baseMaxRadius +
                     statsController.GetSOPlayerStats(E_ModifiersType.SpecialSenseRange).Additive) *
                     statsController.GetSOPlayerStats(E_ModifiersType.SpecialSenseRange).Multiplicative;

        specialSenseUseCount = Mathf.FloorToInt(baseSpecialSenseUseCount +
                               statsController.GetSOPlayerStats(E_ModifiersType.SpecialSenseUseAmount).Additive) *
                               Mathf.FloorToInt(statsController.GetSOPlayerStats(E_ModifiersType.SpecialSenseUseAmount).Multiplicative);
    }

    /// <summary>
    /// Wywołaj z inputu, żeby uruchomić skan.
    /// </summary>
    public void TryUseSpecialSense()
    {
        if (isCooldown) { Debug.Log("[SpecialSense] Cooldown"); return; }
        if (specialSenseUseCount <= 0) { Debug.Log("[SpecialSense] No charges"); return; }

        specialSenseUseCount--;
        pulsePosition = transform.position;
        currentRadius = 0f;

        // Utwórz instancję FMOD, podepnij do obiektu i odpal (intro + loop)
        senseInstance = RuntimeManager.CreateInstance(specialSenseEvent);
        RuntimeManager.AttachInstanceToGameObject(senseInstance, transform);
        senseInstance.start();

        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        pulseCoroutine = StartCoroutine(PulseDetection());
    }

    /* ───────────────────────────── Audio helpers ─────────────────────────────── */
    private void StopSpecialSenseAudio()
    {
        if (senseInstance.isValid())
        {
            senseInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // gra release‑tail
            senseInstance.release();
        }
    }

    /* ───────────────────────────── Pulse logic ───────────────────────────────── */
    private IEnumerator PulseDetection()
    {
        ClearHighlights();
        int linesSpawned = 0;

        while (currentRadius < maxRadius && linesSpawned < maxLinesPerPulse)
        {
            currentRadius += expandSpeed * Time.deltaTime;
            Collider[] hits = Physics.OverlapSphere(pulsePosition, currentRadius, detectionLayers);

            foreach (var hit in hits)
            {
                if (linesSpawned >= maxLinesPerPulse) break;

                GameObject rootObj = hit.transform.root.gameObject;
                if (rootObj.TryGetComponent(out DoorController _)) continue; // pomijamy drzwi
                if (activeHighlights.ContainsKey(rootObj)) continue;

                bool childAlready = false;
                foreach (Transform child in rootObj.transform)
                    if (activeHighlights.ContainsKey(child.gameObject)) { childAlready = true; break; }
                if (childAlready) continue;

                activeHighlights.Add(rootObj, rootObj.layer);
                StartCoroutine(AnimateLineToTarget(rootObj));
                linesSpawned++;
            }
            yield return null;
        }

        yield return new WaitForSeconds(highlightDuration);
        yield return StartCoroutine(ClearHighlightsStepByStep());

        StopSpecialSenseAudio();
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator AnimateLineToTarget(GameObject target)
    {
        LineRenderer lr = lineRendererPrefab ? Instantiate(lineRendererPrefab) : null;
        ParticleSystem ps = particlePrefab ? Instantiate(particlePrefab) : null;

        if (lr)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
        }
        if (ps) ps.Play();

        float duration = Vector3.Distance(transform.position, target.transform.position) / lineSpeed;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Vector3 pos = Vector3.Lerp(transform.position, target.transform.position, t);
            if (lr) lr.SetPosition(1, pos);
            if (ps) ps.transform.position = pos;
            yield return null;
        }

        if (lr) lr.SetPosition(1, target.transform.position);
        if (ps)
        {
            ps.transform.position = target.transform.position;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(ps.gameObject, 1f);
        }

        ToggleHighlight(target, true);
        if (lr) Destroy(lr.gameObject, 0.5f);
    }

    /* ──────────────────────────── Utilities ─────────────────────────────────── */
    private IEnumerator ClearHighlightsStepByStep()
    {
        foreach (var kvp in activeHighlights)
        {
            ToggleHighlight(kvp.Key, false);
            yield return new WaitForSeconds(0.05f);
        }
        activeHighlights.Clear();
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(5f);
        isCooldown = false;
        Debug.Log("[SpecialSense] Ready");
    }

    private void ClearHighlights()
    {
        foreach (var kvp in activeHighlights)
            ToggleHighlight(kvp.Key, false);
        activeHighlights.Clear();
    }

    private void ToggleHighlight(GameObject obj, bool enable)
    {
        if (enable)
        {
            SetLayerRecursively(obj, LayerMaskToLayer(occludedInteractionLayer), detectionLayers);
        }
        else if (activeHighlights.TryGetValue(obj, out int originalLayer))
        {
            SetLayerRecursively(obj, originalLayer, occludedInteractionLayer);
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer, LayerMask filter)
    {
        if (!obj) return;
        if (IsInLayerMask(obj, filter)) obj.layer = newLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer, filter);
    }

    private static bool IsInLayerMask(GameObject obj, LayerMask mask) => ((1 << obj.layer) & mask) != 0;

    private static int LayerMaskToLayer(LayerMask mask)
    {
        int v = mask.value;
        for (int i = 0; i < 32; i++)
            if ((v & (1 << i)) != 0) return i;
        Debug.LogWarning("[SpecialSense] LayerMask has multiple layers, using first.");
        return 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.DrawSphere(transform.position, currentRadius);
    }
#endif
}

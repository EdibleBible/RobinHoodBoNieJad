using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

    Light light;

    public float minIntensity = 0f;
    public float maxIntensity = 1f;
    public int intensitySmoothing = 5;

    public float minRange = 0f;
    public float maxRange = 1f;
    public int rangeSmoothing = 5;

    Queue<float> intensityQueue;
    Queue<float> rangeQueue;
    float lastIntensitySum = 0;
    float lastRangeSum = 0;

    public void Reset()
    {
        intensityQueue.Clear();
        rangeQueue.Clear();
        lastIntensitySum = 0;
        lastRangeSum = 0;
    }

    void Start()
    {
        intensityQueue = new Queue<float>(intensitySmoothing);
        rangeQueue = new Queue<float>(rangeSmoothing);
        light = GetComponent<Light>();
    }

    void Update()
    {
        if (light == null)
            return;

        // Intensity flicker
        while (intensityQueue.Count >= intensitySmoothing)
        {
            lastIntensitySum -= intensityQueue.Dequeue();
        }
        float newIntensity = Random.Range(minIntensity, maxIntensity);
        intensityQueue.Enqueue(newIntensity);
        lastIntensitySum += newIntensity;
        light.intensity = lastIntensitySum / (float)intensityQueue.Count;

        // Range flicker
        while (rangeQueue.Count >= rangeSmoothing)
        {
            lastRangeSum -= rangeQueue.Dequeue();
        }
        float newRange = Random.Range(minRange, maxRange);
        rangeQueue.Enqueue(newRange);
        lastRangeSum += newRange;
        light.range = lastRangeSum / (float)rangeQueue.Count;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {
 
   
    Light light;
   
    public float minIntensity = 0f;
   
    public float maxIntensity = 1f;
 
    public int smoothing = 5;

 
    Queue<float> smoothQueue;
    float lastSum = 0;


    
    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;
    }

    void Start()
    {
        smoothQueue = new Queue<float>(smoothing);
     
        
            light = GetComponent<Light>();
        
    }

    void Update()
    {
        if (light == null)
            return;

       
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        
        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

       
        light.intensity = lastSum / (float)smoothQueue.Count;
    }

}


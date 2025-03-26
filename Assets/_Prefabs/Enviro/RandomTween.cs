using System;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using UnityEngine;

public class RandomTween : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<Transform, FlaotTweenData> objectData;

    private void Awake()
    {
        foreach (var element in objectData)
        {
            element.Key.DOScale(element.Value.MaxScale, element.Value.flickerDuration)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}

[Serializable]
public struct FlaotTweenData
{ 
    public Vector3 MinScale;
    public Vector3 MaxScale;
    public float flickerDuration;
}
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private Transform spikeTransform;
    [SerializeField] private Vector3 extendedPosition;
    [SerializeField] private Vector3 retractedPosition;
    [SerializeField] private float extendDuration = 0.5f;
    [SerializeField] private float retractDuration = 0.7f;
    [SerializeField] private float delayBetweenCycles = 1.0f;

    private void Start()
    {
        ActivateTrapLoop();
    }

    private void ActivateTrapLoop()
    {
        Sequence trapSequence = DOTween.Sequence();
        trapSequence.Append(spikeTransform.DOLocalMove(extendedPosition, extendDuration).SetEase(Ease.OutQuad))
            .AppendInterval(delayBetweenCycles)
            .Append(spikeTransform.DOLocalMove(retractedPosition, retractDuration).SetEase(Ease.InQuad))
            .AppendInterval(delayBetweenCycles)
            .SetLoops(-1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<GameoverController>(out var gameover))
        {
            PlayerBase playerBase = other.GetComponent<PlayerBase>();
            if (!playerBase.PlayerInventory.ItemsInInventory.Any(x => x.ItemType == ItemType.SteelShoes))
                gameover.LoseGame();
        }
    }
}
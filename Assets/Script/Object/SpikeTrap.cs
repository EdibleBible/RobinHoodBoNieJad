using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using FMODUnity;
using FMOD.Studio;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private Transform spikeTransform;
    [SerializeField] private Vector3 extendedPosition;
    [SerializeField] private Vector3 retractedPosition;
    [SerializeField] private float extendDuration = 0.5f;
    [SerializeField] private float retractDuration = 0.7f;
    [SerializeField] private float delayBetweenCycles = 1.0f;

    [SerializeField] private EventReference spikeUpSound;
    [SerializeField] private EventReference spikeDownSound;


    private void Start()
    {
        ActivateTrapLoop();
    }

    private void ActivateTrapLoop()
    {
        Sequence trapSequence = DOTween.Sequence();
        trapSequence.AppendCallback(() => PlaySound(spikeUpSound))
            .Append(spikeTransform.DOLocalMove(extendedPosition, extendDuration).SetEase(Ease.OutQuad))
            .AppendInterval(delayBetweenCycles)
            .AppendCallback(() => PlaySound(spikeDownSound))
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

    private void PlaySound(EventReference soundEvent)
    {
        RuntimeManager.PlayOneShot(soundEvent, transform.position);
    }

}
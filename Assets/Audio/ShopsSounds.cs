using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine.EventSystems;
using UnityEngine;

public class ShopsSounds : MonoBehaviour, IPointerEnterHandler
{
    public EventReference fmodEvent;

    private static EventInstance sharedInstance;
    private static bool isPlaying = false;
    private bool hasPlayedOnce = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fmodEvent.IsNull || isPlaying || hasPlayedOnce)
            return;

        sharedInstance = RuntimeManager.CreateInstance(fmodEvent);
        sharedInstance.start();
        sharedInstance.release();
        isPlaying = true;
        hasPlayedOnce = true;

        sharedInstance.setCallback((FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr eventInst, IntPtr param) =>
        {
            if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED)
            {
                isPlaying = false;
            }

            return FMOD.RESULT.OK;
        });
    }
}

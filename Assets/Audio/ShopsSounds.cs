using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;
using System;

public class ShopsSounds : MonoBehaviour, IPointerEnterHandler
{
    public EventReference fmodEvent;

    private static EventInstance sharedInstance;
    private static bool isPlaying = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fmodEvent.IsNull || isPlaying)
            return;

        sharedInstance = RuntimeManager.CreateInstance(fmodEvent);
        sharedInstance.start();
        sharedInstance.release(); // Cleanup after stop
        isPlaying = true;

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

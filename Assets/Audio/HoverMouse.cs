using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;
using System;

public class HoverMouse : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private string hoverEvent = "event:/MouseUIHover";
    private static EventInstance lastHoverInstance;
    private static bool isHoverSoundPlaying = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHoverSoundPlaying || string.IsNullOrEmpty(hoverEvent)) return;

        lastHoverInstance = RuntimeManager.CreateInstance(hoverEvent);
        lastHoverInstance.start();
        lastHoverInstance.release(); // auto-cleanup
        isHoverSoundPlaying = true;

        lastHoverInstance.setCallback((FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr inst, IntPtr param) =>
        {
            if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED)
            {
                isHoverSoundPlaying = false;
            }

            return FMOD.RESULT.OK;
        });
    }

    public static void OnSceneChange()
    {
        StopLastHoverSound();
    }

    private static void StopLastHoverSound()
    {
        if (lastHoverInstance.isValid())
        {
            lastHoverInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            lastHoverInstance.release();
            isHoverSoundPlaying = false;
        }
    }
}

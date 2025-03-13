using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;

public class HoverMouse : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private string hoverEvent = "event:/MouseUIHover";
    private EventInstance hoverInstance;

    // Globalna referencja do ostatniego d�wi�ku
    private static EventInstance lastHoverInstance;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopLastHoverSound(); // Zatrzymuje poprzedni d�wi�k przed nowym

        hoverInstance = RuntimeManager.CreateInstance(hoverEvent);
        hoverInstance.start();

        lastHoverInstance = hoverInstance; // Ustawiamy nowy d�wi�k jako aktywny
    }

    private static void StopLastHoverSound()
    {
        if (lastHoverInstance.isValid())
        {
            lastHoverInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            lastHoverInstance.release();
        }
    }

    public static void OnSceneChange()
    {
        StopLastHoverSound(); // Zatrzymuje d�wi�k przy przej�ciu na inny canvas
    }
}

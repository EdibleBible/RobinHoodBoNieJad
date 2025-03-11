using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;

public class HoverMouse : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private string hoverEvent = "event:/MouseUIHover";
    private EventInstance hoverInstance;

    // Globalna referencja do ostatniego dŸwiêku
    private static EventInstance lastHoverInstance;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopLastHoverSound(); // Zatrzymuje poprzedni dŸwiêk przed nowym

        hoverInstance = RuntimeManager.CreateInstance(hoverEvent);
        hoverInstance.start();

        lastHoverInstance = hoverInstance; // Ustawiamy nowy dŸwiêk jako aktywny
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
        StopLastHoverSound(); // Zatrzymuje dŸwiêk przy przejœciu na inny canvas
    }
}

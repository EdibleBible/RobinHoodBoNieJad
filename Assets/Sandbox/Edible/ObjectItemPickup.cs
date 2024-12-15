using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent (typeof(InputMouseClick))]
[RequireComponent(typeof(ObjectReachHighlight))]

public class ObjectItemPickup : MonoBehaviour, IMouseLMB, IPlayerReach, IPointerEnterHandler, IPointerExitHandler
{
    public bool isReachable = false;
    private bool isMouseHovering = false;
    public Material materialHighlightOn;
    public Material materialHighlightOff;

    public void MouseClickLeft()
    {
        if (isReachable)
        {
            Debug.Log("dupa2");
        }
    }

    public void IsReachable(bool reachable)
    {
        isReachable = reachable;
        UpdateHighlight();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseHovering = true;
        UpdateHighlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseHovering = false;
        UpdateHighlight();
    }

    void UpdateHighlight()
    {
        if (isReachable && isMouseHovering)
        {
            gameObject.GetComponent<Renderer>().material = materialHighlightOn;
        } else
        {
            gameObject.GetComponent<Renderer>().material = materialHighlightOff;
        }
    }
}

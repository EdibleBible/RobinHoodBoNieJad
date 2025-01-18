using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(ItemBase))]

public class ObjectItemPickup : MonoBehaviour
{
    public bool isReachable = false;
    private bool isMouseHovering = false;
    public Material materialHighlightOn;
    public Material materialHighlightOff;

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

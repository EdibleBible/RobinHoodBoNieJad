using UnityEngine;
using UnityEngine.EventSystems;

public class InputMouseClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            this.gameObject.GetComponent<IMouseLMB>().MouseClickLeft();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            this.gameObject.GetComponent<IMouseRMB>().MouseClickRight();
        }
    }
}

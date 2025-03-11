using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class ClickSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string clickEvent = "event:/MouseUIClick";

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RuntimeManager.PlayOneShot(clickEvent);
        }
    }
}


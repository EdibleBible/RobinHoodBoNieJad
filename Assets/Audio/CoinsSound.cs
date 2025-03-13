
using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class CoinsSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string clickEvent = "event:/CoinsUI";

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RuntimeManager.PlayOneShot(clickEvent);
        }
    }
}



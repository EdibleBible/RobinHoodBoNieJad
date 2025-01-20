using System;
using TMPro;
using UnityEngine;

public class InteractionUiController : MonoBehaviour
{
    [SerializeField] private GameObject interactionGameObject;
    [SerializeField] private TextMeshProUGUI interactionText;

    private void Awake()
    {
        interactionGameObject.SetActive(false);
    }

    public void ToogleInteractionUI(Component sender, object data)
    {
        if (data is not (bool isShow, string textToShow))
        {
            return;
        }
        
        interactionGameObject.SetActive(isShow);
        interactionText.text = textToShow;
    }

}

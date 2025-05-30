using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InteractionUiController : MonoBehaviour
{
    [SerializeField] private GameObject interactionGameObject;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private float interactionUITime;
    private Coroutine showInteractionCoroutine;

    private void Awake()
    {
        interactionGameObject.SetActive(false);
    }

    public void ToogleInteractionUI(Component sender, object data)
    {
        if (data is not (bool isShow, string textToShow, bool haveShowTime))
        {
            return;
        }

        if (isShow && haveShowTime && showInteractionCoroutine == null)
        {
            showInteractionCoroutine = StartCoroutine(InteractionShowTime());
            interactionGameObject.SetActive(isShow);
            interactionText.text = textToShow;
        }
        else if (!haveShowTime && showInteractionCoroutine == null)
        {
            interactionGameObject.SetActive(isShow);
            interactionText.text = textToShow;
        }
    }

    private IEnumerator InteractionShowTime()
    {
        yield return new WaitForSeconds(interactionUITime);
        interactionGameObject.SetActive(false);
        interactionText.text = "";
        showInteractionCoroutine = null;
    }
}
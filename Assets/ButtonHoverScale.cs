using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonHoverScaleDOTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);
    public float duration = 0.2f;

    [Tooltip("Przyciski, które mają się pomniejszać, gdy ten jest kliknięty")]
    public List<ButtonHoverScaleDOTween> otherButtons;

    private Vector3 originalScale;
    private Tween scaleTween;

    private bool isSelected = false;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected)
            return; // jeśli jest kliknięty, nie zmieniaj skali na hover

        scaleTween?.Kill();
        scaleTween = transform.DOScale(hoverScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected)
            return; // jeśli jest kliknięty, nie zmieniaj skali na hover

        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected)
        {
            // odklikuj przycisk (przywróć do oryginalnej skali)
            Deselect();
        }
        else
        {
            // zaznacz ten przycisk
            Select();

            // pomniejsz inne przyciski
            foreach (var btn in otherButtons)
            {
                if (btn != this)
                    btn.Deselect();
            }
        }
    }

    public void Select()
    {
        isSelected = true;
        scaleTween?.Kill();
        scaleTween = transform.DOScale(selectedScale, duration).SetEase(Ease.OutQuad);
    }

    public void Deselect()
    {
        isSelected = false;
        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);
    }
}

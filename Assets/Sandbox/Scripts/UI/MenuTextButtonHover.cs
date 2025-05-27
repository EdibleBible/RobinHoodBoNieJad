using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MenuTextButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public TMP_Text targetText;
    public RectTransform spriteImage;
    public Canvas canvas;

    [Header("Settings")]
    public float scaleFactor = 1.1f;
    public float scaleDuration = 0.2f;
    public Color normalTextColor = Color.white;
    public Color disabledTextColor = Color.gray;

    private RectTransform textRect;
    private Button button;
    private Vector3 originalScale;
    private Tween scaleTween;

    void Start()
    {
        textRect = targetText.rectTransform;
        button = GetComponent<Button>();
        originalScale = textRect.localScale;

        spriteImage.gameObject.SetActive(false);
        UpdateTextColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }

        spriteImage.gameObject.SetActive(true);
        UpdateSpritePosition();

        // Cancel previous tween if any
        scaleTween?.Kill();

        // Animate text scale using DOTween
        scaleTween = textRect.DOScale(originalScale * scaleFactor, scaleDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        spriteImage.gameObject.SetActive(false);

        // Cancel previous tween if any
        scaleTween?.Kill();

        // Animate back to original scale
        scaleTween = textRect.DOScale(originalScale, scaleDuration).SetEase(Ease.InOutQuad);
    }

    private void UpdateSpritePosition()
    {
        if (!button.interactable)
            return;

        Vector2 textSize = targetText.GetRenderedValues(false);
        Vector3 textWorldLeftEdge = textRect.position - textRect.right * (textSize.x * 0.5f);
        float offset = Screen.width * 0.12f;
        Vector3 spriteUIPosition = WorldToCanvasPosition(canvas, textWorldLeftEdge - new Vector3(offset, 0f, 0f));
        spriteImage.position = spriteUIPosition;
    }

    private Vector3 WorldToCanvasPosition(Canvas canvas, Vector3 worldPosition)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return worldPosition;
        }

        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
        Vector3 canvasPosition = new Vector3(
            (viewportPosition.x * canvas.pixelRect.width) - (canvas.pixelRect.width * 0.5f),
            (viewportPosition.y * canvas.pixelRect.height) - (canvas.pixelRect.height * 0.5f),
            0f
        );

        return canvas.transform.TransformPoint(canvasPosition);
    }

    private void UpdateTextColor()
    {
        targetText.color = button.interactable ? normalTextColor : disabledTextColor;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (targetText != null && button != null)
        {
            UpdateTextColor();
        }
    }
#endif
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MenuTextButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text targetText;          // Assign the TMP Text element in the Inspector
    public RectTransform spriteImage;    // Assign the UI Image for the sprite
    public Canvas canvas;                // Assign the Canvas to ensure proper coordinate conversion

    private RectTransform textRect;
    private Button button;

    void Start()
    {
        textRect = targetText.rectTransform;
        spriteImage.gameObject.SetActive(false);
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }
        spriteImage.gameObject.SetActive(true);
        UpdateSpritePosition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        spriteImage.gameObject.SetActive(false);
    }

    private void UpdateSpritePosition()
    {
        // Get the bounds of the rendered text
        Vector2 textSize = targetText.GetRenderedValues(false);

        // Calculate the left edge in world space
        Vector3 textWorldLeftEdge = textRect.position - textRect.right * (textSize.x * 0.5f);

        // Compute the 1% screen width offset
        float offset = Screen.width * 0.07f;

        // Convert world position to UI position relative to the canvas, with 1% left offset
        Vector3 spriteUIPosition = WorldToCanvasPosition(canvas, textWorldLeftEdge - new Vector3(offset, 0f, 0f));

        // Update sprite position
        spriteImage.position = spriteUIPosition;
    }

    private Vector3 WorldToCanvasPosition(Canvas canvas, Vector3 worldPosition)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return worldPosition;
        }

        // Convert world position to viewport point and then to canvas position
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
        Vector3 canvasPosition = new Vector3(
            (viewportPosition.x * canvas.pixelRect.width) - (canvas.pixelRect.width * 0.5f),
            (viewportPosition.y * canvas.pixelRect.height) - (canvas.pixelRect.height * 0.5f),
            0f
        );

        return canvas.transform.TransformPoint(canvasPosition);
    }
}

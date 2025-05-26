using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class MaintainUIElementSize : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 initialSize;
    private Vector3 initialLossyScale;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        CacheInitialValues();
    }

    void CacheInitialValues()
    {
        if (rectTransform == null) return;
        initialSize = rectTransform.rect.size;
        initialLossyScale = rectTransform.lossyScale;
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            // Jeśli coś zmienimy w edytorze, aktualizuj startowe wartości
            CacheInitialValues();
        }
    }

    void LateUpdate()
    {
        Debug.Log("CHUJ");
        if (rectTransform == null || transform.parent == null) return;

        Vector3 parentScale = transform.parent.lossyScale;

        // Odwracamy skalę rodzica, by zneutralizować wpływ na dziecko
        transform.localScale = new Vector3(
            initialLossyScale.x / parentScale.x,
            initialLossyScale.y / parentScale.y,
            initialLossyScale.z / parentScale.z
        );

        // Przywracamy oryginalny rozmiar (ważne dla layoutów/UI)
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialSize.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialSize.y);
    }
}
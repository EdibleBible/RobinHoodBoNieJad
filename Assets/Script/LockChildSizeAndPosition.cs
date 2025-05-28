using UnityEngine;

[ExecuteAlways]
public class LockChildSizeAndPosition : MonoBehaviour
{
    private RectTransform child;
    private Vector3 worldPos;
    private Vector2 size;
    private Quaternion worldRot;

    void OnEnable()
    {
        CacheChild();
        SaveState();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            RestoreState();
        }
    }

    void CacheChild()
    {
        if (transform.childCount == 0) return;
        child = transform.GetChild(0) as RectTransform;
    }

    void SaveState()
    {
        if (child == null) return;
        worldPos = child.position;
        worldRot = child.rotation;
        size = GetWorldSize(child);
    }

    void RestoreState()
    {
        if (child == null) return;

        // Zmiana rodzica mogła zresetować rozmiar — przywracamy oryginalny
        child.position = worldPos;
        child.rotation = worldRot;
        SetWorldSize(child, size);
    }

    Vector2 GetWorldSize(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Vector2(
            Vector3.Distance(corners[0], corners[3]), // wysokość
            Vector3.Distance(corners[0], corners[1])  // szerokość
        );
    }

    void SetWorldSize(RectTransform rt, Vector2 targetSize)
    {
        Vector2 currentSize = GetWorldSize(rt);
        Vector3 scale = rt.localScale;
        if (currentSize.x == 0 || currentSize.y == 0)
            return;

        scale.x *= targetSize.x / currentSize.x;
        scale.y *= targetSize.y / currentSize.y;
        rt.localScale = scale;
    }

    [ContextMenu("Update Lock")]
    public void ManualUpdateLock()
    {
        CacheChild();
        SaveState();
    }
}
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DynamicAimControl : MonoBehaviour
{
    public MultiAimConstraint headConstraint;
    public MultiAimConstraint spineConstraint;
    public Transform target;
    public float headOnlyAngle = 45f;

    void Update()
    {
        if (target == null) return;

        // Oblicz kąt pomiędzy postacią a celem
        Vector3 direction = target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, direction);

        // Dynamiczne ważenie (weight) dla głowy i kręgosłupa
        if (angle <= headOnlyAngle)
        {
            headConstraint.weight = 1f;
            spineConstraint.weight = 0f;
        }
        else
        {
            float normalizedWeight = Mathf.InverseLerp(headOnlyAngle, 180f, angle);
            headConstraint.weight = 1f - normalizedWeight;
            spineConstraint.weight = normalizedWeight;
        }
    }
}
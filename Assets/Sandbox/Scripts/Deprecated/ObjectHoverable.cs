using UnityEngine;

public class ObjectHoverable : MonoBehaviour, IMouseHover
{
    public void OnHoverEnter()
    {
        Debug.Log($"{gameObject.name} - Hover Entered");
        // Add hover effects or logic here
    }

    public void OnHoverExit()
    {
        Debug.Log($"{gameObject.name} - Hover Exited");
        // Remove hover effects or logic here
    }
}


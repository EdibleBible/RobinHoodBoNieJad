using UnityEngine;

public class InputMouseRaycast : MonoBehaviour
{
    public Camera mainCamera; // Assign the camera in the Inspector
    public float rayDistance = 100f;

    private GameObject currentHoveredObject = null;

    void Update()
    {
        // Create a ray from the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform a raycast
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Check if it's a new object or the same one
            if (hitObject != currentHoveredObject)
            {
                // Exit the previous object
                if (currentHoveredObject != null)
                {
                    IMouseHover hoverableExit = currentHoveredObject.GetComponent<IMouseHover>();
                    hoverableExit?.OnHoverExit();
                }

                // Enter the new object
                IMouseHover hoverableEnter = hitObject.GetComponent<IMouseHover>();
                hoverableEnter?.OnHoverEnter();

                currentHoveredObject = hitObject;
            }
        }
        else
        {
            // Exit the currently hovered object if the raycast doesn't hit anything
            if (currentHoveredObject != null)
            {
                IMouseHover hoverableExit = currentHoveredObject.GetComponent<IMouseHover>();
                hoverableExit?.OnHoverExit();

                currentHoveredObject = null;
            }
        }
    }
}


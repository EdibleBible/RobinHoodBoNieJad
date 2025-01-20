using UnityEngine;

public class InputDebugCamera : MonoBehaviour
{
    public float moveSpeed = 5f;      // Speed of camera movement
    public float rotationSpeed = 100f; // Speed of camera rotation
    public float scrollSpeed = 10f;    // Speed of scroll-based movement
    public float smoothTime = 0.2f;  // Smoothing time for movement

    private Vector3 targetPosition;  // Target position for smooth movement
    private Vector3 velocity = Vector3.zero; // Used for smooth damp movement

    private void Start()
    {
        // Initialize the target position to the current position
        targetPosition = transform.position;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleScroll();
        LockZRotation();
    }

    private void HandleMovement()
    {
        // Get input for movement
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;

        // Convert input to world direction based on camera orientation
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0; // Prevent vertical movement
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Calculate the target position
        Vector3 direction = forward * moveZ + right * moveX;
        targetPosition += direction * moveSpeed * Time.deltaTime;

        // Smoothly move the camera
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(2)) // Middle mouse button
        {
            float mouseX = Input.GetAxis("Mouse X"); // Horizontal mouse movement
            float rotationAmountY = mouseX * rotationSpeed * Time.deltaTime;
            // Rotate the camera around the Y-axis
            transform.Rotate(0, rotationAmountY, 0);

            float mouseY = Input.GetAxis("Mouse Y"); // Vertical mouse movement
            float rotationAmountX = mouseY * rotationSpeed * Time.deltaTime;
            // Rotate the camera around the X-axis
            transform.Rotate(-rotationAmountX, 0, 0);
        }
    }

    private void LockZRotation()
    {
        // Ensure the Z rotation is always 0
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0f);
    }
    private void HandleScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f) // Avoid tiny scroll movements
        {
            // Move the camera forward or backward based on scroll input
            Vector3 forward = transform.forward;
            targetPosition += forward * scroll * scrollSpeed;
        }
    }
}


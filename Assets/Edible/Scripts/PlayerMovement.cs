using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player's forward/backward movement
    public float rotationSpeed = 100f; // Speed of the player's rotation

    void Update()
    {
        // Get movement input (W = 1, S = -1, no input = 0)
        float moveInput = Input.GetAxis("Vertical");  // W = 1, S = -1

        // Get rotation input (A = -1, D = 1, no input = 0)
        float rotationInput = Input.GetAxis("Horizontal");  // A = -1, D = 1

        // Move the player forward/backward
        transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime);

        // Rotate the player
        transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
    }
}

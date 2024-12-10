using UnityEngine;

public class InputPlayerWalk : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player's movement
    private Vector3 movement; // Stores movement input

    void Update()
    {
        movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) // North
            movement += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) // South
            movement += Vector3.back;
        if (Input.GetKey(KeyCode.A)) // West
            movement += Vector3.left;
        if (Input.GetKey(KeyCode.D)) // East
            movement += Vector3.right;

        // Normalize the vector to ensure consistent speed in diagonal directions
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
}

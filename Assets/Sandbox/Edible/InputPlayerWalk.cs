using UnityEngine;
[RequireComponent(typeof(CharacterController))]

public class InputPlayerWalk : MonoBehaviour
{
    private float moveSpeed; // Speed of the player's movement
    private float rotationSpeed; // Speed of rotation
    private CharacterController characterController; // Reference to the CharacterController component
    private Vector3 movement; // Stores movement input
    public SOStats stats;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        moveSpeed = stats.playerSpeed;
        rotationSpeed = stats.playerRotationSpeed;
    }

    void Update()
    {
        movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) // North
            movement += new Vector3(-1,0,1);
        if (Input.GetKey(KeyCode.S)) // South
            movement += new Vector3(1, 0, -1);
        if (Input.GetKey(KeyCode.A)) // West
            movement += new Vector3(-1, 0, -1);
        if (Input.GetKey(KeyCode.D)) // East
            movement += new Vector3(1, 0, 1);

        // Normalize the vector to ensure consistent speed in diagonal directions
        movement = movement.normalized * moveSpeed * Time.deltaTime;

        if (movement.magnitude > 0)
        {
            characterController.Move(movement * moveSpeed * Time.deltaTime * 100);
            // Rotate the player towards the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
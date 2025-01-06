using System;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;
[RequireComponent(typeof(CharacterController))]

public class InputPlayerWalk : MonoBehaviour
{
    public static event Action<GameObject> PlayerSendEvent;
    private float moveSpeed; // Speed of the player's movement
    private float rotationSpeed; // Speed of rotation
    private CharacterController characterController; // Reference to the CharacterController component
    private Vector3 movement; // Stores movement input
    public SOStats stats;
    
    // Grawitacja
    private float gravity = -9.81f;  // Stała grawitacyjna
    private float verticalVelocity;   // Prędkość pionowa (grawitacja)
    private float jumpHeight = 2f;    // Wysokość skoku
    private bool isGrounded;          // Flaga, czy gracz stoi na ziemi

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        moveSpeed = stats.playerSpeed;
        rotationSpeed = stats.playerRotationSpeed;

        PlayerSendEvent?.Invoke(gameObject);
    }

    void Update()
    {
        isGrounded = characterController.isGrounded; // Sprawdzamy, czy gracz stoi na ziemi

        movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) // North
            movement += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.S)) // South
            movement += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.A)) // West
            movement += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.D)) // East
            movement += new Vector3(0, 0, 1);

        // Grawitacja - jeśli gracz jest na ziemi, resetujemy pionową prędkość
        if (isGrounded)
        {
            verticalVelocity = -1f; // Mała wartość by gracz nie "skakał" w powietrzu
            if (Input.GetKey(KeyCode.Space)) // Skok
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // Jeśli gracz nie jest na ziemi, stosujemy grawitację
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Dodajemy ruch pionowy (grawitacja i skok)
        movement.y = verticalVelocity * Time.deltaTime;

        if (movement.magnitude > 0)
        {
            characterController.Move(movement * Time.deltaTime);

            // Rotate the player towards the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

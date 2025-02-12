using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class InputPlayerWalk : MonoBehaviour
{
    public static event Action<GameObject> PlayerSendEvent;

    [Header("Movement Parameters")] public float baseMoveSpeed = 5f; // Base speed of the player's movement
    public float baseRotationSpeed = 10f; // Base speed of rotation
    public AnimationCurve accelerationCurve; // Curve for acceleration
    public AnimationCurve decelerationCurve; // Curve for deceleration

    private CharacterController characterController; // Reference to the CharacterController component
    private Vector3 movement; // Stores movement input
    private float currentMoveSpeed; // Current move speed based on acceleration
    private float targetSpeed; // Target speed the player is moving towards

    public SOStats stats;

    // Gravity
    private float gravity = -9.81f; // Gravity constant
    private float verticalVelocity; // Vertical velocity (gravity)
    private float jumpHeight = 2f; // Jump height
    private bool isGrounded; // Is the player on the ground?

    [SerializeField] private float accelerationTime = 1f; // Time to reach max speed
    [SerializeField] private float decelerationTime = 1f; // Time to stop completely
    private float accelerationProgress = 0f; // Progress through the acceleration curve
    private Camera cam;
    private PlayerAnimatorController playerAnimatorController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        baseMoveSpeed = stats.playerSpeed;
        baseRotationSpeed = stats.playerRotationSpeed;

        PlayerSendEvent?.Invoke(gameObject);
        cam = Camera.main;
    }

    void Update()
    {
        isGrounded = characterController.isGrounded; // Check if the player is on the ground

        // Capture input using Unity's Input system
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        // Adjust speed when sprinting (holding Shift)
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            inputMagnitude *= 2;
        }

        // Adjust current speed using acceleration or deceleration curve
        targetSpeed = inputMagnitude * baseMoveSpeed;

        if (targetSpeed > currentMoveSpeed)
        {
            accelerationProgress += Time.deltaTime / accelerationTime;
            currentMoveSpeed = Mathf.Lerp(0, baseMoveSpeed, accelerationCurve.Evaluate(accelerationProgress));
        }
        else if (targetSpeed < currentMoveSpeed)
        {
            accelerationProgress -= Time.deltaTime / decelerationTime;
            currentMoveSpeed = Mathf.Lerp(baseMoveSpeed, 0, decelerationCurve.Evaluate(1 - accelerationProgress));
        }

        // Normalize movement direction relative to camera
        movementDirection = Quaternion.AngleAxis(cam.transform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        // Gravity and jumping logic
        verticalVelocity += gravity * Time.deltaTime;

        if (isGrounded)
        {
            verticalVelocity = -1f; // Small value to keep the player grounded
        }

        // Combine movement and apply gravity
        Vector3 velocity = movementDirection * currentMoveSpeed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
        // Oblicz Velocity X i Z


        // Oblicz bieżące wartości prędkości X i Z
        float velocityX = transform.InverseTransformDirection(characterController.velocity).x;
        float velocityZ = transform.InverseTransformDirection(characterController.velocity).z;
        

        // Rotate the player to face the camera's forward direction
        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0; // Ignore vertical tilt of the camera
        cameraForward.Normalize();

        if (cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, baseRotationSpeed * Time.deltaTime);
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
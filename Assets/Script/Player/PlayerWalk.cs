using System;
using Script.ScriptableObjects;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerWalk : MonoBehaviour
{
    private float accelerationProgress = 0f;
    private float currMoveSpeed = 0f;
    private float targetMoveSpeed = 0f;
    private bool stopMotion = true;

    private Camera camera;

    [Header("Curves")] [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private AnimationCurve decelerationCurve;

    private Vector3 moveDirection = Vector3.zero;
    private float inputMagnitude;

    [Header("Gravity")] 
    [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity;

    private CharacterController characterController;
    [SerializeField] private SOPlayerStatsController playerStatsController;
    
    private float horizontalMovement = 0f;
    private float verticalMovement = 0f;

    [HideInInspector] public bool Sprinting;
    [HideInInspector] public bool Crouching;
    public void SetMotion(bool option)
    {
        stopMotion = option;
    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void SetAxisMovement(Vector2 axis)
    {
        horizontalMovement = axis.x;
        verticalMovement = axis.y;
    }
    
    public void Movement(float speed, float accelerationTime, float decelerationTime, out float xVel, out float yVel)
    {
        if (!stopMotion)
        {
            characterController.Move(Vector3.zero);
            xVel = 0;
            yVel = 0;
            return;
        }

        moveDirection = new Vector3(horizontalMovement, 0, verticalMovement);
        inputMagnitude = Mathf.Clamp01(moveDirection.magnitude);
        speed = (speed + playerStatsController.GetSOPlayerStats(E_ModifiersType.PlayerSpeed).Additive) *
                playerStatsController.GetSOPlayerStats(E_ModifiersType.PlayerSpeed).Multiplicative;

        targetMoveSpeed = inputMagnitude * speed;
        
        if (targetMoveSpeed > currMoveSpeed)
        {
            accelerationProgress += Time.deltaTime / (accelerationTime + playerStatsController.GetSOPlayerStats(E_ModifiersType.Acceleration).Additive) * playerStatsController.GetSOPlayerStats(E_ModifiersType.Acceleration).Multiplicative;
            currMoveSpeed = Mathf.Lerp(0, speed, accelerationCurve.Evaluate(accelerationProgress));
        }
        else if (targetMoveSpeed < currMoveSpeed)
        {
            accelerationProgress -= Time.deltaTime / decelerationTime;
            currMoveSpeed = Mathf.Lerp(speed, 0, decelerationCurve.Evaluate(1 - accelerationProgress));
        }

        // Normalize movement direction relative to camera
        moveDirection = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * moveDirection;
        moveDirection.Normalize();

        verticalVelocity += gravity * Time.deltaTime;

        if (characterController.isGrounded)
        {
            verticalVelocity = -1f; // Small value to keep the player grounded
        }

        Vector3 velocity = moveDirection * currMoveSpeed;
        velocity.y = verticalVelocity;

        // Local velocity relative to character's transform
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        xVel = localVelocity.x;
        yVel = localVelocity.z;

        characterController.Move(velocity * Time.deltaTime);

    }

    public Vector3 GetCharacterVelocity()
    {
        return characterController.velocity;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerWalk : MonoBehaviour
{
    private float accelerationProgress = 0f;
    private float currMoveSpeed = 0f;
    private float targetMoveSpeed = 0f;

    private Camera camera;

    [Header("Curves")] [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private AnimationCurve decelerationCurve;

    private Vector3 moveDirection = Vector3.zero;
    private float inputMagnitude;

    [Header("Gravity")] [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Movement(float speed, float accelerationTime, float decelerationTime)
    {
        //Read Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical);
        inputMagnitude = Mathf.Clamp01(moveDirection.magnitude);

        targetMoveSpeed = inputMagnitude * speed;


        if (targetMoveSpeed > currMoveSpeed)
        {
            accelerationProgress += Time.deltaTime / accelerationTime;
            currMoveSpeed = Mathf.Lerp(0, speed, accelerationCurve.Evaluate(accelerationProgress));
        }
        else if (targetMoveSpeed < currMoveSpeed)
        {
            accelerationProgress -= Time.deltaTime / decelerationTime;
            currMoveSpeed = Mathf.Lerp(speed, 0, decelerationCurve.Evaluate(1 - accelerationProgress));
        }

        // Normalize movement direction relative to camera
        moveDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * moveDirection;
        moveDirection.Normalize();

        verticalVelocity += gravity * Time.deltaTime;

        if (characterController.isGrounded)
        {
            verticalVelocity = -1f; // Small value to keep the player grounded
        }

        Vector3 velocity = moveDirection * currMoveSpeed;
        velocity.y = verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
        
        
        Debug.Log("velocity: " + characterController.velocity);
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

using System;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimatorController))]
public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private Camera camera;
    private float cameraPlayerDistance;
    private PlayerAnimatorController animatorController;

    [SerializeField] private Transform debugLookPoint;
    [SerializeField] private Vector3 lookPointOffset;
    [SerializeField] private LayerMask objectLayer;

    [SerializeField] private float rotationSpeed = 45f;
    private float currAngleToRotateThreshold = 0;
    [SerializeField] private float angleToRotateThresholdStay = 45f;
    [SerializeField] private float angleToRotateThresholdMoveForward = 15f;
    [SerializeField] private float angleToRotateThresholdMoveBackward = 0f;

    private bool isRotating = false;

    private void Awake()
    {
        if (camera == null)
            camera = Camera.main;
        animatorController = GetComponent<PlayerAnimatorController>(); // Pobieramy kontroler animacji
    }

    public void UpdateRotation(float velocity = 0f)
    {
        if (velocity > 0)
        {
            currAngleToRotateThreshold = angleToRotateThresholdMoveForward;
        }
        else if (velocity < 0)
        {
            currAngleToRotateThreshold = angleToRotateThresholdMoveBackward;
        }
        else if (velocity == 0)
        {
            currAngleToRotateThreshold = angleToRotateThresholdStay;
        }


        cameraPlayerDistance = Vector3.Distance(transform.position, camera.transform.position);
        float cameraDistanceOffset = cameraPlayerDistance * 1.5f;

        if (Physics.Raycast(camera.transform.position + lookPointOffset, camera.transform.forward + lookPointOffset,
                out RaycastHit hit,
                cameraDistanceOffset, objectLayer))
        {
            Debug.DrawRay(camera.transform.position + lookPointOffset,
                camera.transform.forward * hit.distance + lookPointOffset, Color.red);
            debugLookPoint.position = hit.point;
        }
        else
        {
            Debug.DrawRay(camera.transform.position + lookPointOffset,
                camera.transform.forward * cameraDistanceOffset + lookPointOffset, Color.yellow);
            debugLookPoint.position = camera.transform.position + (camera.transform.forward * cameraDistanceOffset);
        }

        Vector3 cameraForward = camera.transform.forward;
        cameraForward.y = 0; // Ignore vertical tilt of the camera
        cameraForward.Normalize();

        if (cameraForward != Vector3.zero)
        {
            // Obliczamy kąt między kierunkiem kamery a kierunkiem postaci
            float angle = Vector3.Angle(transform.forward, cameraForward);
            Debug.DrawRay(transform.position, cameraForward * angle, Color.green);

            if (angle > currAngleToRotateThreshold)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
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
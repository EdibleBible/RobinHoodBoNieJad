using System;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimatorController))]
public class PlayerRotation : MonoBehaviour
{
    private Camera camera;
    private float cameraPlayerDistance;
    private PlayerAnimatorController animatorController;

    [SerializeField] private Transform debugLookPoint;
    [SerializeField] private LayerMask objectLayer;

    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private float angleToRotateThreshold = 15f; // Próg kąta w stopniach

    private bool isRotating = false;

    private void Awake()
    {
        camera = Camera.main;
        animatorController = GetComponent<PlayerAnimatorController>(); // Pobieramy kontroler animacji
    }

    private void Update()
    {
        cameraPlayerDistance = Vector3.Distance(transform.position, camera.transform.position);
        float cameraDistanceOffset = cameraPlayerDistance * 1.5f;

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit,
                cameraDistanceOffset, objectLayer))
        {
            Debug.DrawRay(camera.transform.position, camera.transform.forward * hit.distance, Color.red);
            debugLookPoint.position = hit.point;
        }
        else
        {
            Debug.DrawRay(camera.transform.position, camera.transform.forward * cameraDistanceOffset, Color.yellow);
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

            if (angle > angleToRotateThreshold)
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
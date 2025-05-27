using Cinemachine;
using UnityEngine;
using DG.Tweening; // DOTween

[RequireComponent(typeof(PlayerAnimatorController))]
public class PlayerRotation : MonoBehaviour
{
    [Header("Ustawienia Cinemachine")]
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [Header("Ustawienia obrotu kamery")]
    [SerializeField]
    private float mouseSensitivityX = 100f;
    [SerializeField]
    private float mouseSensitivityY = 100f;
    [SerializeField]
    private bool invertY = false;
    [SerializeField]
    private float minPitch = -90f;
    [SerializeField]
    private float maxPitch = 90f;

    [Header("Ustawienia obrotu gracza")]
    [SerializeField]
    private float playerRotationSpeed = 720f;
    [SerializeField]
    private float angleThreshold = 5f;
    [SerializeField]
    private float rotationSmoothTime = 0.1f;

    [Header("Ustawienia kamery kucania")]
    [SerializeField]
    private Vector3 crouchOffset = new Vector3(0, -0.5f, 0);
    [SerializeField]
    private float crouchTransitionTime = 0.3f;

    [SerializeField]
    private Transform camTransform;
    private float pitch = 0f;
    private float yaw = 0f;
    private float currentYawVelocity = 0f;

    private Vector3 originalCamLocalPos;
    private Tween cameraTween;

    public bool CanRotation;

    private void Start()
    {
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;

        // Zapamiętujemy oryginalną pozycję kamery
        originalCamLocalPos = camTransform.localPosition;
    }

    // Metoda wywoływana z Update() z zewnątrz, która obsługuje obrót kamery i gracza
    public void UpdateRotation()
    {
        if(!CanRotation)
            return;
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        yaw += mouseX;
        pitch += invertY ? mouseY : -mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Ustawiamy obrót kamery (pitch)
        camTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Obliczamy docelowy obrót gracza (yaw)
        Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

        if (angleDifference > angleThreshold)
        {
            float smoothedYaw =
                Mathf.SmoothDampAngle(transform.eulerAngles.y, yaw, ref currentYawVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothedYaw, 0f);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }

    // Metoda wywoływana przy zmianie stanu kucania (np. w zależności od wejścia gracza)
    public void SetCrouch(bool isCrouching)
    {
        // Zabezpieczenie przed spamowaniem: przerywamy bieżący tween, jeśli istnieje
        if (cameraTween != null && cameraTween.IsActive())
        {
            cameraTween.Kill();
        }

        // Docelowa pozycja kamery zależna od stanu kucania
        Vector3 targetPos = isCrouching ? originalCamLocalPos + crouchOffset : originalCamLocalPos;

        // Jeśli kamera już znajduje się w docelowej pozycji, nie rozpoczynamy tweeningu
        if (camTransform.localPosition == targetPos) return;

        cameraTween = camTransform.DOLocalMove(targetPos, crouchTransitionTime)
            .SetEase(Ease.OutSine);
    }
}
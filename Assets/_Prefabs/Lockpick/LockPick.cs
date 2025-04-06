using UnityEngine;

public class LockPick : MonoBehaviour
{
    [SerializeField]
    private Canvas screenCoverCanvas;
    [HideInInspector]
    public Camera cam;
    public Transform innerLock;
    public Transform pickPosition;

    public float maxAngle = 90;
    public float lockSpeed = 10;

    [Range(1, 25)]
    public float lockRange = 10;
    public float sensitivity = 2;

    private float eulerAngle;
    private float unlockAngle;
    private Vector2 unlockRange;

    private float keyPressTime = 0;

    private bool movePick = true;

    private ILockPick objectToLockPick = null;
    private PlayerStateMachineController playerStateMachine;
    [SerializeField]
    private KeyCode rotateButton;

    void Start()
    {
        newLock();
    }

    public void SetUpCamera(Camera cam)
    {
        this.cam = cam;
        screenCoverCanvas.worldCamera = cam;
        screenCoverCanvas.gameObject.SetActive(true);
    }

    void Update()
    {
        if (cam == null)
            return;

        transform.localPosition = pickPosition.localPosition;
        if (movePick)
        {
            Vector3 right = cam.transform.right;
            float mouseDelta = Input.GetAxis("Mouse X") * sensitivity;
            eulerAngle += mouseDelta * Mathf.Sign(Vector3.Dot(right, -cam.transform.right));
            eulerAngle = Mathf.Clamp(eulerAngle, -maxAngle, maxAngle);

            Quaternion rotateTo = Quaternion.AngleAxis(eulerAngle, Vector3.forward);
            transform.localRotation = rotateTo;
        }

        if (Input.GetKeyDown(rotateButton))
        {
            movePick = false;
            keyPressTime = 1;
        }

        if (Input.GetKeyUp(rotateButton))
        {
            movePick = true;
            keyPressTime = 0;
        }

        float percentage = Mathf.Clamp01(1 - Mathf.Abs((eulerAngle - unlockAngle) / 100));
        float lockRotation = (percentage * maxAngle) * keyPressTime;
        float maxRotation = percentage * maxAngle;

        // Użycie LerpAngle zamiast Lerp
        float lockLerp = Mathf.LerpAngle(innerLock.localEulerAngles.z, lockRotation, Time.deltaTime * lockSpeed);
        innerLock.localEulerAngles = new Vector3(0, 0, lockLerp);

        if (Mathf.Abs(Mathf.DeltaAngle(lockLerp, maxRotation)) <= 1)
        {
            if (eulerAngle < unlockRange.y && eulerAngle > unlockRange.x)
            {
                objectToLockPick.UnlockLock();
                objectToLockPick.StopInteracting();
                movePick = true;
                keyPressTime = 0;
            }
            else
            {
                float randomRotation = Random.Range(-5f, 5f);
                transform.rotation *= Quaternion.Euler(0, 0, randomRotation);
            }
        }
    }

    public void SetObjectToLockPick(ILockPick objectToLockPick)
    {
        this.objectToLockPick = objectToLockPick;
    }

    void newLock()
    {
        unlockAngle = Random.Range(-maxAngle + lockRange, maxAngle - lockRange);
        unlockRange = new Vector2(unlockAngle - lockRange, unlockAngle + lockRange);
    }
}
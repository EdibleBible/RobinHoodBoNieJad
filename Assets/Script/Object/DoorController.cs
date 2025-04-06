using System;
using UnityEngine;
using DG.Tweening;
using FMODUnity;
using FMOD.Studio;

public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Bools")]
    public bool TwoSideInteraction
    {
        get => twoSideInteraction;
        set => twoSideInteraction = value;
    }

    public bool twoSideInteraction;
    public bool CanInteract { get; set; } = true;
    public bool IsBlocked
    {
        get => isBlocked;
        set => isBlocked = value;
    }

    public bool isBlocked;


    [Header("Open Setting")]
    [SerializeField]
    public float openTime;

    public AnimationCurve openCurve;

    public float closeTime;
    public AnimationCurve closeCurve;
    public LayerMask playerLayer;

    [Header("LeftDoor")] public GameObject doorPivotLeft;
    public Vector3 leftDoorClosePosition;
    public float leftDoorAngle;
    public Vector3 endLeftDoorOpenPosition;

    [Header("RightDoor")] public GameObject doorPivotRight;
    public Vector3 rightDoorClosePosition;
    public float rightDoorAngle;
    public Vector3 endRightDoorOpenPosition;

    [Header("FMOD")] public EventReference doorsEvent;
    public Transform soundSource;
    public EventInstance doorSoundInstance;

    [Header("Events")] public GameEvent showUIEvent;
    public GameEvent interactEvent;

    public GameEvent ShowUIEvent
    {
        get => showUIEvent;
        set => showUIEvent = value;
    }

    public GameEvent InteractEvent
    {
        get => interactEvent;
        set => interactEvent = value;
    }

    [Header("Callbacks")]
    public string InteractMessage
    {
        get => interactMessage;
        set => interactMessage = value;
    }

    public string interactMessage;

    public string BlockedMessage
    {
        get => blockedMessage;
        set => blockedMessage = value;
    }

    public string blockedMessage;

    [Header("Debug")] public bool isDebug;
    public KeyCode debugKey = KeyCode.C;

    public bool IsUsed { get; set; } = false;
    public Tween isDoorOpenTween;

    public virtual void Interact(Transform player)
    {
        Debug.Log($"Interact called - IsBlocked: {IsBlocked}, CanInteract: {CanInteract}, isDoorOpenTween: {isDoorOpenTween}, IsUsed: {IsUsed}");

        if (IsBlocked)
        {
            ShowUIEvent.Raise(this, (true, BlockedMessage, true));
            return;
        }

        if (!CanInteract)
        {
            Debug.Log("Cannot interact, CanInteract is false.");
            return;
        }

        if (isDoorOpenTween != null)
        {
            Debug.Log("Stopping current animation.");
            isDoorOpenTween.Kill();
            isDoorOpenTween = null;
        }

        InteractEvent.Raise(this, null);

        if (!IsUsed)
        {
            IsUsed = true;
            CheckPlayerPosition();
            PlayDoorSound("Open");

            Debug.Log("Opening doors.");

            Tween leftDoorTween = doorPivotLeft.transform.DOLocalRotate(endLeftDoorOpenPosition, openTime)
                .SetEase(openCurve);
            Tween rightDoorTween = doorPivotRight.transform.DOLocalRotate(endRightDoorOpenPosition, openTime)
                .SetEase(openCurve);

            isDoorOpenTween = DOTween.Sequence()
                .Join(leftDoorTween)
                .Join(rightDoorTween)
                .OnComplete(() =>
                {
                    Debug.Log("Doors opened completely.");
                    isDoorOpenTween = null;
                    if (!TwoSideInteraction) CanInteract = false;
                });
        }
        else
        {
            IsUsed = false;
            PlayDoorSound("Close");

            Debug.Log("Closing doors.");

            Tween leftDoorTween = doorPivotLeft.transform.DOLocalRotate(leftDoorClosePosition, closeTime)
                .SetEase(closeCurve);
            Tween rightDoorTween = doorPivotRight.transform.DOLocalRotate(rightDoorClosePosition, closeTime)
                .SetEase(closeCurve);

            isDoorOpenTween = DOTween.Sequence()
                .Join(leftDoorTween)
                .Join(rightDoorTween)
                .OnComplete(() =>
                {
                    Debug.Log("Doors closed completely.");
                    isDoorOpenTween = null;
                });
        }
    }
    public void CheckPlayerPosition()
    {
        // Ustawienia BoxCast
        float boxWidth = 0.1f; // Szerokość boxa (dostosuj do swoich potrzeb)
        float boxHeight = 0.1f; // Wysokość boxa (dostosuj do swoich potrzeb)
        float boxDepth = 0.05f; // Głębokość boxa (krótkie rzutowanie wzdłuż osi)
        Vector3 boxHalfExtents = new Vector3(boxWidth / 2f, boxHeight / 2f, boxDepth / 2f); // Półwymiary boxa

        RaycastHit hit;

        // Debugowanie, wyświetlanie kierunku rzutowania
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.right * 10f, Color.yellow, 5f);

        // BoxCast z początkowym punktem transform.position, kierunkiem Vector3.right, rozmiarem boxa
        bool isHit = Physics.BoxCast(transform.position + new Vector3(0, 0.5f, 0), boxHalfExtents, transform.right,
            out hit, Quaternion.identity, 10f, playerLayer);

        // Debugowanie BoxCast
        if (isHit)
        {
            Debug.Log("Player detected in BoxCast!");

            // Rysowanie linii reprezentujących boxa w przestrzeni
            DrawBoxDebug(transform.position + new Vector3(0, 0.5f, 0), boxHalfExtents, transform.right);

            // Jeśli wykryto gracza, ustaw odpowiednie pozycje otwierania drzwi
            endRightDoorOpenPosition = new Vector3(0, -rightDoorAngle, 0);
            endLeftDoorOpenPosition = new Vector3(0, leftDoorAngle, 0);

            // Debuguj informacje o trafionym obiekcie
            Debug.Log($"Hit object: {hit.collider.gameObject.name}");
            Debug.Log($"Hit point: {hit.point}");
        }
        else
        {
            Debug.Log("Player not detected in BoxCast!");

            // Jeśli nie wykryto gracza, ustaw inne pozycje
            endRightDoorOpenPosition = new Vector3(0, rightDoorAngle, 0);
            endLeftDoorOpenPosition = new Vector3(0, -leftDoorAngle, 0);


            // Rysowanie linii reprezentujących boxa w przestrzeni, gdy nie ma trafienia
            DrawBoxDebug(transform.position + new Vector3(0, 0.5f, 0), boxHalfExtents, transform.right);
        }
    }

    // Funkcja rysująca box w przestrzeni w celu debugowania
    public void DrawBoxDebug(Vector3 origin, Vector3 halfExtents, Vector3 direction)
    {
        Vector3 frontRight = origin + direction * 10f + new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
        Vector3 frontLeft = origin + direction * 10f + new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
        Vector3 backRight = origin + direction * 10f + new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
        Vector3 backLeft = origin + direction * 10f + new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
        Vector3 frontRightLower = origin + direction * 10f + new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
        Vector3 frontLeftLower = origin + direction * 10f + new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
        Vector3 backRightLower = origin + direction * 10f + new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
        Vector3 backLeftLower = origin + direction * 10f + new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);

        // Rysowanie boków boxa
        Debug.DrawLine(frontRight, frontLeft, Color.blue, 5f);
        Debug.DrawLine(frontLeft, backLeft, Color.blue, 5f);
        Debug.DrawLine(backLeft, backRight, Color.blue, 5f);
        Debug.DrawLine(backRight, frontRight, Color.blue, 5f);
        Debug.DrawLine(frontRightLower, frontLeftLower, Color.blue, 5f);
        Debug.DrawLine(frontLeftLower, backLeftLower, Color.blue, 5f);
        Debug.DrawLine(backLeftLower, backRightLower, Color.blue, 5f);
        Debug.DrawLine(backRightLower, frontRightLower, Color.blue, 5f);
        Debug.DrawLine(frontRight, frontRightLower, Color.green, 5f);
        Debug.DrawLine(frontLeft, frontLeftLower, Color.green, 5f);
        Debug.DrawLine(backRight, backRightLower, Color.green, 5f);
        Debug.DrawLine(backLeft, backLeftLower, Color.green, 5f);
    }


    public void PlayDoorSound(string doorState, float volume = 0.2f)
    {
        // Jeśli istnieje instancja dźwięku, zatrzymaj ją przed stworzeniem nowej
        if (doorSoundInstance.isValid())
        {
            doorSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            doorSoundInstance.release();
        }

        // Tworzenie nowej instancji dźwięku
        doorSoundInstance = FMODUnity.RuntimeManager.CreateInstance(doorsEvent);
        doorSoundInstance.setParameterByNameWithLabel("Door", doorState);

        // Ustawienie głośności
        doorSoundInstance.setVolume(volume);

        // Ustawienie dźwięku jako 2D
        //doorSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Vector3.zero));
        //doorSoundInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, 1.0f);
        //doorSoundInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, 15.0f);

        doorSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));

        // Uruchomienie dźwięku
        doorSoundInstance.start();
    }


    public virtual void ShowUI()
    {
        ShowUIEvent.Raise(this, (true, InteractMessage, false));
    }

    public virtual void HideUI()
    {
        ShowUIEvent.Raise(this, (false, "", false));
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.right * 0.7f);
        Gizmos.DrawSphere(transform.position + transform.right * 0.7f, 0.1f);
    }
}
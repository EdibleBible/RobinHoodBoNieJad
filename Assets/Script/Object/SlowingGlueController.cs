using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using Script.ScriptableObjects;
using UnityEngine;

public class SlowingGlueController : MonoBehaviour, IInteractable, IStatChangeable
{
    public bool ChangedStats { get; set; }
    public List<StatParameters> StatsToChange
    {
        get => statToChange;
        set => statToChange = value;
    }

    public List<StatParameters> statToChange;

    [Header("FMOD")]
    [SerializeField] private EventReference pressSoundEvent;  // D�wi�k wej�cia w paj�czyn�
    [SerializeField] private EventReference releaseSoundEvent; // D�wi�k wyj�cia z paj�czyny
    [SerializeField] private EventReference webMovementEvent;  // D�wi�k poruszania si� w paj�czynie
    [SerializeField] private Transform soundSource;

    private EventInstance pressSoundInstance;
    private EventInstance releaseSoundInstance;
    private EventInstance webMovementInstance;

    private bool playerInWeb = false;
    private Rigidbody playerRb;
    private bool isPlaying = false;

    public void StartSlowingGlue(SOPlayerStatsController statToControll)
    {
        AddModifier(statToControll);

        // Odtwórz dźwięk wejścia do pajęczyny
        pressSoundInstance = RuntimeManager.CreateInstance(pressSoundEvent);
        RuntimeManager.AttachInstanceToGameObject(pressSoundInstance, soundSource);
        pressSoundInstance.start();

        // Inicjalizuj dźwięk ruchu w pajęczynie (ale nie uruchamiaj jeszcze)
        webMovementInstance = RuntimeManager.CreateInstance(webMovementEvent);
        RuntimeManager.AttachInstanceToGameObject(webMovementInstance, soundSource);

        // Logowanie stanu początkowego parametru Movement
        Debug.Log("Initial Movement value: 0");
        webMovementInstance.setParameterByName("Movement", 0f); // Możesz ustawić początkowy stan jako 0
    }


    public void StopSlowingGlue(SOPlayerStatsController statToControll)
    {
        RemoveModifier(statToControll);

        // Odtw�rz d�wi�k wyj�cia z paj�czyny
        releaseSoundInstance = RuntimeManager.CreateInstance(releaseSoundEvent);
        RuntimeManager.AttachInstanceToGameObject(releaseSoundInstance, soundSource);
        releaseSoundInstance.start();

        // Zatrzymaj d�wi�k ruchu w paj�czynie
        webMovementInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isPlaying = false;
    }

    public void AddModifier(SOPlayerStatsController statToControll)
    {
        if (StatsToChange == null)
            return;

        if (!ChangedStats)
        {
            foreach (var stat in StatsToChange)
            {
                statToControll.ChangeModifier(stat.Additive, stat.Multiplicative, stat.ModifierType);
            }
            ChangedStats = true;
        }
    }

    public void RemoveModifier(SOPlayerStatsController statToControll)
    {
        if (StatsToChange == null)
            return;

        if (ChangedStats)
        {
            foreach (var stat in StatsToChange)
            {
                statToControll.ChangeModifier(-stat.Additive, -stat.Multiplicative, stat.ModifierType);
            }
            ChangedStats = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerBase>(out PlayerBase playerBase))
            return;

        playerInWeb = true;
        playerRb = other.GetComponent<Rigidbody>();

        StartSlowingGlue(playerBase.PlayerStatsController);
    }

    public void OnTriggerStay(Collider other)
    {
        if (!playerInWeb || !other.CompareTag("Player"))
            return;


        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("Brak CharacterController na obiekcie gracza!");
            return;
        }

        float movement = controller.velocity.magnitude > 0.1f ? 1f : 0f;


        Debug.Log("Movement: " + movement);
        webMovementInstance.setParameterByName("Movement", movement);

        if (movement > 0 && !isPlaying)
        {
            Debug.Log("Starting web movement sound.");
            webMovementInstance.start();
            isPlaying = true;
        }
        else if (movement == 0 && isPlaying)
        {
            Debug.Log("Stopping web movement sound.");
            webMovementInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<PlayerBase>(out PlayerBase playerBase))
            return;

        StopSlowingGlue(playerBase.PlayerStatsController);
        playerInWeb = false;
    }

    public void Interact(Transform player) { }
    public void ShowUI() { }
    public void HideUI() { }

    [HideInInspector] public GameEvent ShowUIEvent { get; set; }
    [HideInInspector] public GameEvent InteractEvent { get; set; }
    [HideInInspector] public bool CanInteract { get; set; }
    [HideInInspector] public bool IsBlocked { get; set; }
    [HideInInspector] public bool TwoSideInteraction { get; set; }
    [HideInInspector] public string InteractMessage { get; set; }
    [HideInInspector] public string BlockedMessage { get; set; }
    [HideInInspector] public bool IsUsed { get; set; }
}

using UnityEngine;
using FMODUnity;

public class PlayerFootsteps : MonoBehaviour
{
    public string footstepEventPath = "event:/PlayerFootsteps"; // Œcie¿ka do eventu FMOD
    private CharacterController characterController;

    [SerializeField] private float stepInterval = 0.5f; // Czas miêdzy krokami (dopasuj do tempa animacji)
    private float stepTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // SprawdŸ, czy postaæ siê porusza i jest na ziemi
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime;

            // Jeœli up³yn¹³ czas dla kolejnego kroku, odtwórz dŸwiêk
            if (stepTimer >= stepInterval)
            {
                PlayFootstepSound();
                stepTimer = 0f; // Resetuj licznik czasu
            }
        }
        else
        {
            // Resetuj licznik czasu, gdy postaæ siê nie porusza
            stepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        // Odtwórz dŸwiêk kroków w pozycji postaci
        RuntimeManager.PlayOneShot(footstepEventPath, transform.position);
    }
}


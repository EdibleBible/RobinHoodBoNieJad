using UnityEngine;
using FMODUnity;

public class PlayerFootsteps : MonoBehaviour
{
    public string footstepEventPath = "event:/PlayerFootsteps"; // �cie�ka do eventu FMOD
    private CharacterController characterController;

    [SerializeField] private float stepInterval = 0.5f; // Czas mi�dzy krokami (dopasuj do tempa animacji)
    private float stepTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Sprawd�, czy posta� si� porusza i jest na ziemi
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime;

            // Je�li up�yn�� czas dla kolejnego kroku, odtw�rz d�wi�k
            if (stepTimer >= stepInterval)
            {
                PlayFootstepSound();
                stepTimer = 0f; // Resetuj licznik czasu
            }
        }
        else
        {
            // Resetuj licznik czasu, gdy posta� si� nie porusza
            stepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        // Odtw�rz d�wi�k krok�w w pozycji postaci
        RuntimeManager.PlayOneShot(footstepEventPath, transform.position);
    }
}


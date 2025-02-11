using UnityEngine;

public class LevelCameraFollow : MonoBehaviour
{
    public GameObject player;         // Reference to the player transform
    public Vector3 offset = new Vector3(2, 2, 0); // Offset from the player
    public float followSpeed = 5f;  // Speed of the camera following the player
    public float rotationSpeed = 10f; // Speed of the camera aligning with the player

    private void OnEnable()
    {
        InputPlayerWalk.PlayerSendEvent += AssignPlayer;
    }

    private void OnDisable()
    {
        InputPlayerWalk.PlayerSendEvent -= AssignPlayer;
    }

    private void AssignPlayer(GameObject playerObject)
    {
        player = playerObject;
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned in CameraFollow script.");
            return;
        }

        FollowPlayer();
        RotateTowardsPlayer();
    }

    private void FollowPlayer()
    {
        // Calculate the target position based on the player's position and the offset
        Vector3 targetPosition = player.transform.position + offset;

        // Smoothly interpolate the camera's position to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void RotateTowardsPlayer()
    {
        // Get the direction to look at the player
        Vector3 directionToPlayer = player.transform.position - transform.position;

        // Smoothly rotate the camera to face the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}

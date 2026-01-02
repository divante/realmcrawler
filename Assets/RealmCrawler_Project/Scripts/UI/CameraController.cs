using UnityEngine;

public class CameraController : MonoBehaviour
{
  [SerializeField] private Transform playerTransform; // Assign this in the Inspector
  [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -20f); // Adjust as needed

  void LateUpdate()
  {
    if (playerTransform != null)
    {
      // Calculate the desired camera position
      Vector3 targetPosition = playerTransform.position + offset;

      // Apply smoothing to the camera movement
      transform.position = Vector3.Lerp(transform.position, targetPosition, 5f * Time.deltaTime);

      // Ensure the camera always looks at the player
      transform.LookAt(playerTransform);
    }
  }
}
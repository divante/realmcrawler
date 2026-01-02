using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

  [SerializeField] private ICharacterPhysics physics;

  public float rotationSpeed = 5f;

  Vector2 moveInput;
  Vector2 mousePosition;

  void Start()
  {
    physics = GetComponent<ICharacterPhysics>();
  }

  void Update()
  {
    LookAtMouse();
  }

  public void OnMove(InputAction.CallbackContext context)
  {
    moveInput = context.ReadValue<Vector2>();
    physics.TryMove(moveInput);
  }

  public void OnMouseMove(InputAction.CallbackContext context)
  {
    mousePosition = context.ReadValue<Vector2>();

  }

  private void LookAtMouse()
  {
    Ray cameraRay = Camera.main.ScreenPointToRay(mousePosition);
    RaycastHit cameraRayHit;

    if (!Physics.Raycast(cameraRay, out cameraRayHit))
      return;

    Vector3 targetPosition = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
    // Calculate the target rotation
    Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);

    // Smoothly interpolate towards the target rotation over time
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
  }
}



using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ICharacterController
{
  public event Action<Vector2> OnMoveEvent;
  public event Action<Vector2> OnLookEvent;

  public float rotationSpeed = 5f;

  public Vector2 currentMoveInput { get; private set; }
  public Vector2 mousePosition { get; private set; }

  // private StateMachine stateMachine;

  void Start()
  {
    // stateMachine = new StateMachine();
    // stateMachine.Initialize(new IdleState(gameObject));
  }

  void Update()
  {
    // stateMachine.Update();
    HandleRotation();
  }

  void FixedUpdate()
  {
    // stateMachine.FixedUpdate();
  }

  public void OnMove(InputAction.CallbackContext context)
  {
    currentMoveInput = context.ReadValue<Vector2>();
    OnMoveEvent?.Invoke(currentMoveInput);
  }

  public void OnMouseMove(InputAction.CallbackContext context)
  {
    mousePosition = context.ReadValue<Vector2>();
    OnLookEvent?.Invoke(mousePosition);
  }

  public void HandleRotation()
  {
    Ray cameraRay = Camera.main.ScreenPointToRay(mousePosition);
    RaycastHit cameraRayHit;

    if (!Physics.Raycast(cameraRay, out cameraRayHit))
      return;

    Vector3 targetPosition = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
    Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);

    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
  }
}
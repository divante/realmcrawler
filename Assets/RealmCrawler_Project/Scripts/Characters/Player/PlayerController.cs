using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ICharacterController
{
  public event Action<Vector2> MoveEvent;
  public event Action<Quaternion> LookEvent;

  public event Action AttackLightEvent;
  public event Action AttackHeavyEvent;
  public event Action<int> SpellCastPressedEvent;
  public event Action<int> SpellCastReleasedEvent;

  [SerializeField] private float _rotationSpeed = 5f;
  private Vector2 _currentMoveInput;
  private Vector2 _prevMoveInput;
  private Vector2 _mousePosition;

  private Quaternion _lookDirection;
  private Quaternion _prevLookDirection;

  public Vector2 GetMovement() => _currentMoveInput;
  public Quaternion GetLookDirection() => _lookDirection;

  public void OnMove(InputAction.CallbackContext context)
  {
    _currentMoveInput = context.ReadValue<Vector2>();
    if (_currentMoveInput == _prevMoveInput) return;

    MoveEvent?.Invoke(_currentMoveInput);
    _prevMoveInput = _currentMoveInput;
  }

  public void OnMouseMove(InputAction.CallbackContext context)
  {
    _mousePosition = context.ReadValue<Vector2>();

    Ray cameraRay = Camera.main.ScreenPointToRay(_mousePosition);
    RaycastHit cameraRayHit;

    if (!Physics.Raycast(cameraRay, out cameraRayHit))
      return;

    // get GameObject this script is attached to


    Vector3 targetPosition = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
    _lookDirection = Quaternion.LookRotation(targetPosition - transform.position);

    if (_lookDirection != null && _lookDirection == _prevLookDirection) return;

    LookEvent?.Invoke(_lookDirection);
    _prevLookDirection = _lookDirection;
  }

  public void OnAttackLight(InputAction.CallbackContext context)
  {
    if (context.started) AttackLightEvent?.Invoke();
  }

  public void OnAttackHeavy(InputAction.CallbackContext context)
  {
    if (context.started) AttackHeavyEvent?.Invoke();
  }

  public void OnSpell1(InputAction.CallbackContext context) => HandleSpellInput(context, 0);
  public void OnSpell2(InputAction.CallbackContext context) => HandleSpellInput(context, 1);
  public void OnSpell3(InputAction.CallbackContext context) => HandleSpellInput(context, 2);
  public void OnSpell4(InputAction.CallbackContext context) => HandleSpellInput(context, 3);

  private void HandleSpellInput(InputAction.CallbackContext context, int slot)
  {
    if (context.started)   SpellCastPressedEvent?.Invoke(slot);
    if (context.canceled)  SpellCastReleasedEvent?.Invoke(slot);
  }

  public void Disable()
  {
  }

  public void Enable()
  {
  }
}
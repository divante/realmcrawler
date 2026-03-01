using UnityEngine;

public class IsMovingCondition : ConditionBase
{
  private bool _wasValidLastFrame = false;
  [SerializeField] private bool _invert = false;

  private ICharacterController _controller;

  public override void Initialize(StateRuntime state)
  {
    base.Initialize(state);
    _controller = _state.Owner.GetComponent<ICharacterController>();
    if (_controller == null)
    {
      Debug.LogError("No Character Controller found on the controlled object");
    }
  }

  public override void Activate(StateDefinition stateDefinition)
  {
    base.Activate(stateDefinition);

    _isValid = !_controller.GetMovement().Equals(Vector2.zero) ^ _invert;
    _wasValidLastFrame = _isValid;

    _controller.MoveEvent += OnMove;
  }

  public override void Deactivate(StateDefinition stateDefinition)
  {
    base.Deactivate(stateDefinition);
    _controller.MoveEvent -= OnMove;
  }

  private void OnMove(Vector2 movement)
  {
    _isValid = !movement.Equals(Vector2.zero) ^ _invert;

    if (_isValid && !_wasValidLastFrame) NotifyConditionMet();
    _wasValidLastFrame = _isValid;
  }

}

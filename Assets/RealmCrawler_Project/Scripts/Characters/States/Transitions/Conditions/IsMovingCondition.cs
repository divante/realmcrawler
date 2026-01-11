using System;
using Unity.VisualScripting;
using UnityEngine;

public class IsMovingCondition : ConditionBase
{
  private bool _isMoving = false;
  private bool _wasMovingLastFrame = false;

  public override bool Evaluate() => _isMoving;

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
    _controller.OnMoveEvent += OnMove;
    Debug.Log("Condition activated and awaiting for character to move..");
  }

  public override void Deactivate(StateDefinition stateDefinition)
  {
    base.Deactivate(stateDefinition);
    _controller.OnMoveEvent -= OnMove;
  }

  private void OnMove(Vector2 movement)
  {
    Debug.Log("Character is moving! " + movement);
    _isMoving = !movement.Equals(Vector2.zero);
    if (_isMoving && !_wasMovingLastFrame)
      NotifyConditionMet();
    _wasMovingLastFrame = _isMoving;
  }

}

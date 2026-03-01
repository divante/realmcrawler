using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class MoveAction : ActionRuntime
{
	private ICharacterPhysics _physics;
	private ICharacterController _controller;
	private Vector2 _moveDirection = Vector2.zero;

	public override void Initialize(ActionDefinition definition, GameObject owner)
	{
		base.Initialize(definition, owner);
		_physics = _owner.GetComponent<ICharacterPhysics>();
		_controller = _owner.GetComponent<ICharacterController>();
	}

	public override void Activate()
	{
		base.Activate();
		_controller.MoveEvent += OnMove;

		_moveDirection = _controller.GetMovement();
		_physics.TryMove(_moveDirection);
	}

	private void OnMove(Vector2 movement)
	{
		_moveDirection = movement;
		_physics.TryMove(_moveDirection);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		_controller.MoveEvent -= OnMove;
		_moveDirection = Vector2.zero;
		_physics.StopMove();
	}
}

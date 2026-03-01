using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class LookAction : ActionRuntime
{
	private ICharacterController _controller;
	private Quaternion _lookDirection;

	public override void Initialize(ActionDefinition definition, GameObject owner)
	{
		base.Initialize(definition, owner);
		_controller = _owner.GetComponent<ICharacterController>();
		_lookDirection = _owner.transform.rotation;
	}

	public override void Activate()
	{
		base.Activate();
		_controller.LookEvent += OnLook;
	}

	private void OnLook(Quaternion lookDirection)
	{
		_lookDirection = lookDirection;

	}

	public override void Deactivate()
	{
		base.Deactivate();
		_controller.LookEvent -= OnLook;
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		_owner.transform.rotation = Quaternion.Slerp(_owner.transform.rotation, _lookDirection, 5f * Time.deltaTime);
	}
}

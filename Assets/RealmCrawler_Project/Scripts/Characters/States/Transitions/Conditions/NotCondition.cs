using System;
using Unity.VisualScripting;
using UnityEngine;

public class NotCondition : ConditionBase
{
	[SelectImplementation]
	[SerializeReference, Tooltip("Condition that must be false for this transition.")]
	public ConditionBase condition;

	public override void Initialize(StateRuntime state)
	{
		base.Initialize(state);
		condition?.Initialize(state);
	}

	public override void Update()
	{
		if (condition.Evaluate() || _isValid) return;
		_isValid = true;
		NotifyConditionMet();
	}
}

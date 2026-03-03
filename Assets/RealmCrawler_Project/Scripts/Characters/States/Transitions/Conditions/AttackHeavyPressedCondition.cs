using System;
using UnityEngine;

[Serializable]
public class AttackHeavyPressedCondition : ConditionBase
{
    private ICharacterController _controller;

    public override void Initialize(StateRuntime state)
    {
        base.Initialize(state);
        _controller = _state.Owner.GetComponent<ICharacterController>();
    }

    public override void Activate(StateDefinition stateDefinition)
    {
        base.Activate(stateDefinition);
        _controller.AttackHeavyEvent += OnAttackHeavy;
    }

    public override void Deactivate(StateDefinition stateDefinition)
    {
        base.Deactivate(stateDefinition);
        _controller.AttackHeavyEvent -= OnAttackHeavy;
    }

    private void OnAttackHeavy() => NotifyConditionMet();
}

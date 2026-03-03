using System;
using UnityEngine;

[Serializable]
public class AttackLightPressedCondition : ConditionBase
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
        _controller.AttackLightEvent += OnAttackLight;
    }

    public override void Deactivate(StateDefinition stateDefinition)
    {
        base.Deactivate(stateDefinition);
        _controller.AttackLightEvent -= OnAttackLight;
    }

    private void OnAttackLight() => NotifyConditionMet();
}

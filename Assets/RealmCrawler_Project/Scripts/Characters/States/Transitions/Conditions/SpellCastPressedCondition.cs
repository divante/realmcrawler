using System;
using UnityEngine;

[Serializable]
public class SpellCastPressedCondition : ConditionBase
{
    [SerializeField] private int slot = 0;

    private ICharacterController _controller;

    public override void Initialize(StateRuntime state)
    {
        base.Initialize(state);
        _controller = _state.Owner.GetComponent<ICharacterController>();
    }

    public override void Activate(StateDefinition stateDefinition)
    {
        base.Activate(stateDefinition);
        _controller.SpellCastPressedEvent += OnSpellCastPressed;
    }

    public override void Deactivate(StateDefinition stateDefinition)
    {
        base.Deactivate(stateDefinition);
        _controller.SpellCastPressedEvent -= OnSpellCastPressed;
    }

    private void OnSpellCastPressed(int pressedSlot)
    {
        if (pressedSlot == slot) NotifyConditionMet();
    }
}

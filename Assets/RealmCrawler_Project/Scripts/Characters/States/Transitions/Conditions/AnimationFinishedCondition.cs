using System;
using UnityEngine;

[Serializable]
public class AnimationFinishedCondition : ConditionBase
{
    private AnimationEventBridge _bridge;

    public override void Initialize(StateRuntime state)
    {
        base.Initialize(state);
        _bridge = _state.Owner.GetComponentInChildren<AnimationEventBridge>();
        if (_bridge == null)
            Debug.LogError("[AnimationFinishedCondition] No AnimationEventBridge found.", _state.Owner);
    }

    public override void Activate(StateDefinition stateDefinition)
    {
        base.Activate(stateDefinition);
        _bridge.OnAnimationFinished += OnAnimationFinished;
    }

    public override void Deactivate(StateDefinition stateDefinition)
    {
        base.Deactivate(stateDefinition);
        _bridge.OnAnimationFinished -= OnAnimationFinished;
    }

    private void OnAnimationFinished() => NotifyConditionMet();
}

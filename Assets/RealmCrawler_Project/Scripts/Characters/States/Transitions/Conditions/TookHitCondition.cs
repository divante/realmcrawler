using System;
using UnityEngine;

/// <summary>
/// Fires when the character receives a hit.
/// Hook up IDamageable.OnHitReceived (or equivalent) once the health/damage system is in place.
/// </summary>
[Serializable]
public class TookHitCondition : ConditionBase
{
    // TODO: replace with the actual damage/health interface once it exists
    // private IDamageable _damageable;

    public override void Initialize(StateRuntime state)
    {
        base.Initialize(state);
        // _damageable = _state.Owner.GetComponent<IDamageable>();
    }

    public override void Activate(StateDefinition stateDefinition)
    {
        base.Activate(stateDefinition);
        // _damageable.OnHitReceived += OnHitReceived;
    }

    public override void Deactivate(StateDefinition stateDefinition)
    {
        base.Deactivate(stateDefinition);
        // _damageable.OnHitReceived -= OnHitReceived;
    }

    // Called by the damage system
    public void OnHitReceived() => NotifyConditionMet();
}

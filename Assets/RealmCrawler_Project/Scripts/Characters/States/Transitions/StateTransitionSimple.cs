using UnityEngine;

/// <summary>
/// Simple transition that checks a single condition.
/// </summary>
public sealed class SimpleStateTransition : StateTransitionBase
{
  [SelectImplementation]
  [SerializeReference, Tooltip("Condition that must be true for this transition.")]
  public ConditionBase condition;

  public override void Initialize(StateRuntime state)
  {
    base.Initialize(state);
    condition?.Initialize(state);
  }

  public override void Activate(StateDefinition stateDefinition)
  {
    base.Activate(stateDefinition);
    condition.ConditionMet += OnConditionMet;
  }

  public override void Deactivate(StateDefinition stateDefinition)
  {
    base.Deactivate(stateDefinition);
    if (condition != null) condition.ConditionMet -= OnConditionMet;
  }

  private void OnConditionMet() => NotifyTransitionRequested();

}
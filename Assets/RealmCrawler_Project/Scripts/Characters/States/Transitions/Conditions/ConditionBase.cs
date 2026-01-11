using System;
using UnityEngine;

/// <summary>
/// A simple condition that can be edited in the inspector.
/// </summary>
[Serializable]
public abstract class ConditionBase
{
  [DefaultFileName]
  [SerializeField]
  public string conditionName;

  public event Action ConditionMet;

  protected StateRuntime _state;

  public virtual bool Evaluate()
  {
    return false;
  }

  public virtual void Initialize(StateRuntime state)
  {
    _state = state;
    _state.OnStateEnter += Activate;
    _state.OnStateExit += Deactivate;
  }

  public virtual void Activate(StateDefinition stateDefinition) { }

  public virtual void Deactivate(StateDefinition stateDefinition) => ConditionMet = null;

  protected void NotifyConditionMet() => ConditionMet?.Invoke();

}

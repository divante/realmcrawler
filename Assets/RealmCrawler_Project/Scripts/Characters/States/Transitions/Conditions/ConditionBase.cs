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
  public string _conditionName;

  public event Action ConditionMet;

  protected StateRuntime _state;
  protected bool _isValid = false;

  public virtual bool Evaluate() => _isValid;

  public virtual void Initialize(StateRuntime state)
  {
    _state = state;
    _state.OnStateEnter += Activate;
    _state.OnStateExit += Deactivate;
    // Debug.Log("Initialized condition " + _conditionName);

  }

  public virtual void Activate(StateDefinition stateDefinition) { }

  public virtual void Deactivate(StateDefinition stateDefinition) => ConditionMet = null;

  protected void NotifyConditionMet() => ConditionMet?.Invoke();

  public virtual void Update() { }
  public virtual void FixedUpdate() { }

}

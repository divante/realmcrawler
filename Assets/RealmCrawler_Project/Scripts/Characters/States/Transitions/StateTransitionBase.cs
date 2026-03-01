using System;
using UnityEngine;

/// <summary>
/// The abstract base class that every concrete transition type inherits from.
/// Its kept as abstract to allow transitionsto have more complex condition logic
/// if needed.
/// </summary>
[Serializable]
public abstract class StateTransitionBase : ICloneable
{

  public event Action<StateDefinition> TransitionRequested;

  [SerializeField]
  [DefaultFileName]
  private string _transitionName;
  public string TransitionName => _transitionName;

  [SerializeReference, Tooltip("Target state for this transition.")]
  public StateDefinition targetState;

  protected StateRuntime _state;

  public virtual void Initialize(StateRuntime state)
  {
    _state = state;
    _state.OnStateEnter += Activate;
    _state.OnStateExit += Deactivate;
    Debug.Log("Initialized transition " + _transitionName);
  }

  public virtual void Activate(StateDefinition stateDefinition) { }

  public virtual void Deactivate(StateDefinition stateDefinition) => TransitionRequested = null;

  protected void NotifyTransitionRequested() => TransitionRequested?.Invoke(targetState);

  public object Clone()
  {
    string json = JsonUtility.ToJson(this);
    return JsonUtility.FromJson(json, GetType());
  }

  public virtual void Update() { }
  public virtual void FixedUpdate() { }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StateRuntime : IStateRuntime
{
  public event Action<StateDefinition> OnStateEnter;
  public event Action<StateDefinition> OnStateExit;

  protected GameObject _controlledObject { get; private set; }
  public GameObject Owner => _controlledObject;

  private StateDefinition _currentDef;
  private List<StateTransitionBase> _activeTransitions = new();


  public StateRuntime(GameObject controlledObject, StateDefinition startingState)
  {
    _controlledObject = controlledObject;
    Debug.Log("instantiated " + startingState.stateName);
    Enter(startingState);
  }

  virtual public void Enter(StateDefinition nextDef)
  {
    if (nextDef == null) { Debug.LogError("Enter called with null StateDefinition"); return; }

    _currentDef = nextDef;
    foreach (var defTransition in _currentDef.transitions)
    {
      var runtimeTrans = (StateTransitionBase)defTransition.Clone();
      runtimeTrans.Initialize(this);

      runtimeTrans.TransitionRequested += OnTransitionRequested;

      _activeTransitions.Add(runtimeTrans);

      Debug.Log("Initializing transition " + runtimeTrans.TransitionName + " for state " + _currentDef.stateName);
      Debug.Log("Transition target state: " + runtimeTrans.targetState.stateName);
    }
    OnStateEnter?.Invoke(_currentDef);
  }

  public virtual void Exit()
  {
    foreach (var rt in _activeTransitions)
      rt.TransitionRequested -= OnTransitionRequested;
    // do these in seperate steps to ensure there is no conflict/concurrency
    foreach (var rt in _activeTransitions)
      rt.Deactivate(_currentDef);

    _activeTransitions.Clear();


    OnStateExit?.Invoke(_currentDef);
    _currentDef = null;
  }

  public void OnTransitionRequested(StateDefinition newState)
  {
    _controlledObject.GetComponent<MonoBehaviour>().StartCoroutine(DoTransition(newState));
  }

  private IEnumerator DoTransition(StateDefinition targetDef)
  {
    Exit();
    yield return null;
    Enter(targetDef);
  }

  public virtual void Update() { }

  public virtual void FixedUpdate() { }

}
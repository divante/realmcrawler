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
  private List<ActionRuntime> _activeActions = new();


  public StateRuntime(GameObject controlledObject, StateDefinition startingState)
  {
    _controlledObject = controlledObject;
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
    }

    foreach (var actionDef in _currentDef.actions)
    {
      var actionRuntime = actionDef.Runtime(_controlledObject);
      _activeActions.Add(actionRuntime);
      actionRuntime.Activate();
    }

    OnStateEnter?.Invoke(_currentDef);
    Debug.Log("Entering state " + _currentDef.stateName);
  }

  public virtual void Exit()
  {
    foreach (var rt in _activeTransitions)
      rt.Deactivate(_currentDef);
    _activeTransitions.Clear();

    foreach (var action in _activeActions)
      action.Deactivate();
    _activeActions.Clear();

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

  public virtual void Update()
  {
    foreach (StateTransitionBase rt in _activeTransitions) rt.Update();
    foreach (ActionRuntime rt in _activeActions) rt.Update();
  }

  public virtual void FixedUpdate()
  {
    foreach (StateTransitionBase rt in _activeTransitions) rt.FixedUpdate();
    foreach (ActionRuntime rt in _activeActions) rt.FixedUpdate();
  }

}
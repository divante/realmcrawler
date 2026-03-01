using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
  [SerializeField] private StateDefinition _startingState;

  private StateRuntime _runtime;

  public event Action<StateDefinition> RequestTransition;

  private void Awake()
  {
    if (_startingState == null)
      Debug.LogError("[StateMachine] No starting state assigned!", this);
    else
      _runtime = new StateRuntime(gameObject, _startingState);

  }

  public void Update()
  {
    _runtime?.Update();
  }

  public void FixedUpdate()
  {
    _runtime?.FixedUpdate();
  }


}
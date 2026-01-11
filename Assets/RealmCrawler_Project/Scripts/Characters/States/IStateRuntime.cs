using System;
using UnityEngine;

public interface IStateRuntime
{
    event Action<StateDefinition> OnStateEnter;
    event Action<StateDefinition> OnStateExit;

    void Enter(StateDefinition def);
    void Exit();

    void Update();
    void FixedUpdate();

}
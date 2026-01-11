using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private StateDefinition _startingState;

    private StateRuntime _runtime;

    // Expose transition request for external listeners (e.g., UI)
    public event Action<StateDefinition> RequestTransition;

    private void Awake()
    {
        if (_startingState == null)
            Debug.LogError("[StateMachine] No starting state assigned!", this);
        else
            _runtime = new StateRuntime(gameObject, _startingState);
    }

    // Forward Unity callbacks to the runtime so that states can react each frame.
    private void Update()
    {
        _runtime?.Update();
    }

    private void FixedUpdate()
    {
        _runtime?.FixedUpdate();
    }

    /// <summary>
    /// Called by input handling scripts to trigger a basic attack (index 0 or 1).
    /// </summary>
    public void PerformBasicAttack(int buttonIndex)
    {
        // Placeholder: broadcast via RequestTransition if needed.
        Debug.Log($"Performing basic attack #{buttonIndex}");
    }

    /// <summary>
    /// Called to cast a spell (0‑3).
    /// </summary>
    public void CastSpell(int spellIndex)
    {
        // Placeholder for future implementation.
        Debug.Log($"Casting spell #{spellIndex}");
    }
}
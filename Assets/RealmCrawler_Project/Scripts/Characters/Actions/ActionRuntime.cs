using System;
using UnityEngine;

namespace RealmCrawler.Characters.Actions
{
    /// <summary>
    /// Base class for all action runtime execution.
    /// Handles action phases: Windup → Execute → Recovery → Completed
    /// </summary>
    [Serializable]
    public class ActionRuntime : ICloneable
    {
        public enum ActionPhase
        {
            Idle,
            Windup,      // Preparation (can cancel)
            Execute,     // Active (hit detection, effects)
            Recovery,    // Can't interrupt
            Completed    // Done
        }

        public event Action<ActionPhase> OnPhaseChanged;
        public event Action OnActionCompleted;
        public event Action OnActionCancelled;

        protected ActionDefinition _definition;
        protected GameObject _owner { get; private set; }
        protected ActionPhase _currentPhase { get; private set; } = ActionPhase.Idle;
        protected float _phaseTimer;
        protected bool _isCancelable;
        protected bool _isInterruptible;

        /// <summary>
        /// Current phase of the action
        /// </summary>
        public ActionPhase CurrentPhase => _currentPhase;

        /// <summary>
        /// Whether the action can be cancelled
        /// </summary>
        public bool IsCancelable => _isCancelable;

        /// <summary>
        /// Whether the action can be interrupted by other actions
        /// </summary>
        public bool IsInterruptible => _isInterruptible;

        /// <summary>
        /// Whether the action is currently running
        /// </summary>
        public bool IsRunning => _currentPhase != ActionPhase.Idle && _currentPhase != ActionPhase.Completed;

        /// <summary>
        /// Initialize the action runtime with definition and owner
        /// </summary>
        public virtual void Initialize(ActionDefinition definition, GameObject owner)
        {
            _definition = definition;
            _owner = owner;
            _currentPhase = ActionPhase.Idle;
            _phaseTimer = 0f;
        }

        /// <summary>
        /// Start the action execution
        /// </summary>
        public virtual void StartAction()
        {
            if (_currentPhase != ActionPhase.Idle)
            {
                Debug.LogWarning($"[{_definition.actionName}] Action already running in phase {_currentPhase}");
                return;
            }

            EnterPhase(ActionPhase.Windup);
        }

        /// <summary>
        /// Cancel the action (only works during cancelable phases)
        /// </summary>
        public virtual void CancelAction()
        {
            if (!_isCancelable)
            {
                Debug.LogWarning($"[{_definition.actionName}] Cannot cancel action in phase {_currentPhase}");
                return;
            }

            OnActionCancelled?.Invoke();
            ResetAction();
        }

        /// <summary>
        /// Complete the action (called when all phases finish)
        /// </summary>
        public virtual void CompleteAction()
        {
            EnterPhase(ActionPhase.Completed);
            OnActionCompleted?.Invoke();
        }

        /// <summary>
        /// Reset the action to idle state
        /// </summary>
        protected virtual void ResetAction()
        {
            _currentPhase = ActionPhase.Idle;
            _phaseTimer = 0f;
            OnPhaseChanged?.Invoke(ActionPhase.Idle);
        }

        /// <summary>
        /// Enter a new phase and notify listeners
        /// </summary>
        protected virtual void EnterPhase(ActionPhase newPhase)
        {
            _currentPhase = newPhase;
            _phaseTimer = 0f;

            // Set cancel/interrupt flags based on phase
            _isCancelable = newPhase == ActionPhase.Windup;
            _isInterruptible = newPhase == ActionPhase.Windup;

            OnPhaseChanged?.Invoke(newPhase);
            Debug.Log($"[{_definition.actionName}] Entered phase: {newPhase}");
        }

        /// <summary>
        /// Advance to the next phase
        /// </summary>
        protected virtual void AdvancePhase()
        {
            switch (_currentPhase)
            {
                case ActionPhase.Windup:
                    EnterPhase(ActionPhase.Execute);
                    break;
                case ActionPhase.Execute:
                    EnterPhase(ActionPhase.Recovery);
                    break;
                case ActionPhase.Recovery:
                    CompleteAction();
                    break;
                default:
                    Debug.LogWarning($"[{_definition.actionName}] Cannot advance from phase {_currentPhase}");
                    break;
            }
        }

        /// <summary>
        /// Activate the action (called by StateRuntime when entering state)
        /// </summary>
        public virtual void Activate() { }

        /// <summary>
        /// Deactivate the action (called by StateRuntime when exiting state)
        /// </summary>
        public virtual void Deactivate()
        {
            ResetAction();
        }

        /// <summary>
        /// Update the action (called every frame)
        /// </summary>
        public virtual void Update()
        {
            if (_currentPhase == ActionPhase.Idle || _currentPhase == ActionPhase.Completed)
                return;

            _phaseTimer += Time.deltaTime;
            OnUpdate();
        }

        /// <summary>
        /// FixedUpdate for physics operations
        /// </summary>
        public virtual void FixedUpdate()
        {
            if (_currentPhase == ActionPhase.Idle || _currentPhase == ActionPhase.Completed)
                return;

            OnFixedUpdate();
        }

        /// <summary>
        /// Override this to handle per-frame logic during action
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// Override this for physics operations
        /// </summary>
        protected virtual void OnFixedUpdate() { }

        /// <summary>
        /// Clone the action runtime for state isolation
        /// </summary>
        public object Clone()
        {
            string json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson(json, GetType());
        }
    }
}

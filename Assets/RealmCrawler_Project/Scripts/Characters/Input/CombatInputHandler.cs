using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RealmCrawler.Characters.Input
{
    /// <summary>
    /// Handles all combat input (attacks, spells, weapon switching)
    /// Supports both Keyboard+Mouse and Controller input
    /// </summary>
    public class CombatInputHandler : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Fired when player requests a fast attack
        /// </summary>
        public event Action<AttackType> OnAttackRequested;

        /// <summary>
        /// Fired when player requests a spell (0-3 for spell slots)
        /// </summary>
        public event Action<int> OnSpellRequested;

        /// <summary>
        /// Fired when player requests to switch to next weapon
        /// </summary>
        public event Action OnNextWeaponRequested;

        /// <summary>
        /// Fired when player requests to switch to previous weapon
        /// </summary>
        public event Action OnPreviousWeaponRequested;

        #endregion

        #region Serialized Fields

        [SerializeField]
        private InputActionAsset _inputActionAsset;

        [SerializeField, Tooltip("Whether to enable input on start")]
        private bool _enableOnStart = true;

        #endregion

        #region Private Fields

        private InputActionMap _combatMap;
        private InputAction _fastAttackAction;
        private InputAction _heavyAttackAction;
        private InputAction _spell1Action;
        private InputAction _spell2Action;
        private InputAction _spell3Action;
        private InputAction _spell4Action;
        private InputAction _switchWeaponNextAction;
        private InputAction _switchWeaponPrevAction;

        private bool _isInputEnabled;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeInputActions();
        }

        private void OnEnable()
        {
            if (_enableOnStart)
                EnableInput();
        }

        private void OnDisable()
        {
            DisableInput();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes references to input actions from the asset
        /// </summary>
        private void InitializeInputActions()
        {
            if (_inputActionAsset == null)
            {
                Debug.LogError("[CombatInputHandler] No InputActionAsset assigned!", this);
                return;
            }

            _combatMap = _inputActionAsset.FindActionMap("Combat");
            if (_combatMap == null)
            {
                Debug.LogError("[CombatInputHandler] 'Combat' action map not found!", this);
                return;
            }

            // Attack actions
            _fastAttackAction = _combatMap.FindAction("FastAttack");
            _heavyAttackAction = _combatMap.FindAction("HeavyAttack");

            // Spell actions
            _spell1Action = _combatMap.FindAction("Spell1");
            _spell2Action = _combatMap.FindAction("Spell2");
            _spell3Action = _combatMap.FindAction("Spell3");
            _spell4Action = _combatMap.FindAction("Spell4");

            // Weapon switching
            _switchWeaponNextAction = _combatMap.FindAction("SwitchWeaponNext");
            _switchWeaponPrevAction = _combatMap.FindAction("SwitchWeaponPrev");

            // Validate all actions are found
            ValidateInputActions();
        }

        private void ValidateInputActions()
        {
            bool allValid = true;

            allValid &= ValidateAction(_fastAttackAction, "FastAttack");
            allValid &= ValidateAction(_heavyAttackAction, "HeavyAttack");
            allValid &= ValidateAction(_spell1Action, "Spell1");
            allValid &= ValidateAction(_spell2Action, "Spell2");
            allValid &= ValidateAction(_spell3Action, "Spell3");
            allValid &= ValidateAction(_spell4Action, "Spell4");
            allValid &= ValidateAction(_switchWeaponNextAction, "SwitchWeaponNext");
            allValid &= ValidateAction(_switchWeaponPrevAction, "SwitchWeaponPrev");

            if (!allValid)
            {
                Debug.LogError("[CombatInputHandler] Some input actions are missing. Check your InputActionAsset.");
            }
        }

        private bool ValidateAction(InputAction action, string actionName)
        {
            if (action == null)
            {
                Debug.LogError($"[CombatInputHandler] Action '{actionName}' not found!");
                return false;
            }
            return true;
        }

        #endregion

        #region Input Control

        /// <summary>
        /// Enables combat input
        /// </summary>
        public void EnableInput()
        {
            if (_combatMap != null)
            {
                _combatMap.Enable();
                _isInputEnabled = true;
                Debug.Log("[CombatInputHandler] Input enabled");
            }
        }

        /// <summary>
        /// Disables combat input
        /// </summary>
        public void DisableInput()
        {
            if (_combatMap != null)
            {
                _combatMap.Disable();
                _isInputEnabled = false;
                Debug.Log("[CombatInputHandler] Input disabled");
            }
        }

        /// <summary>
        /// Toggles input on/off
        /// </summary>
        public void ToggleInput(bool enabled)
        {
            if (enabled)
                EnableInput();
            else
                DisableInput();
        }

        #endregion

        #region Input Processing

        /// <summary>
        /// Called when fast attack input is detected
        /// </summary>
        private void OnFastAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnAttackRequested?.Invoke(AttackType.Fast);
            }
        }

        /// <summary>
        /// Called when heavy attack input is detected
        /// </summary>
        private void OnHeavyAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnAttackRequested?.Invoke(AttackType.Heavy);
            }
        }

        /// <summary>
        /// Called when spell 1 input is detected
        /// </summary>
        private void OnSpell1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSpellRequested?.Invoke(0);
            }
        }

        /// <summary>
        /// Called when spell 2 input is detected
        /// </summary>
        private void OnSpell2(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSpellRequested?.Invoke(1);
            }
        }

        /// <summary>
        /// Called when spell 3 input is detected
        /// </summary>
        private void OnSpell3(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSpellRequested?.Invoke(2);
            }
        }

        /// <summary>
        /// Called when spell 4 input is detected
        /// </summary>
        private void OnSpell4(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSpellRequested?.Invoke(3);
            }
        }

        /// <summary>
        /// Called when switch to next weapon input is detected
        /// </summary>
        private void OnSwitchWeaponNext(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnNextWeaponRequested?.Invoke();
            }
        }

        /// <summary>
        /// Called when switch to previous weapon input is detected
        /// </summary>
        private void OnSwitchWeaponPrev(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPreviousWeaponRequested?.Invoke();
            }
        }

        #endregion

        #region Input Binding (Editor)

        /// <summary>
        /// Binds input actions to their callbacks
        /// Called automatically if using Unity Input System's generated bindings
        /// </summary>
        private void BindInputActions()
        {
            _fastAttackAction?.performed += OnFastAttack;
            _heavyAttackAction?.performed += OnHeavyAttack;
            _spell1Action?.performed += OnSpell1;
            _spell2Action?.performed += OnSpell2;
            _spell3Action?.performed += OnSpell3;
            _spell4Action?.performed += OnSpell4;
            _switchWeaponNextAction?.performed += OnSwitchWeaponNext;
            _switchWeaponPrevAction?.performed += OnSwitchWeaponPrev;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Checks if input is currently enabled
        /// </summary>
        public bool IsInputEnabled() => _isInputEnabled;

        /// <summary>
        /// Manually trigger an attack (for testing or scripted sequences)
        /// </summary>
        public void TriggerAttack(AttackType type)
        {
            OnAttackRequested?.Invoke(type);
        }

        /// <summary>
        /// Manually trigger a spell (for testing or scripted sequences)
        /// </summary>
        public void TriggerSpell(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex <= 3)
                OnSpellRequested?.Invoke(slotIndex);
        }

        #endregion
    }

    /// <summary>
    /// Types of attacks available
    /// </summary>
    public enum AttackType
    {
        Fast,
        Heavy
    }
}

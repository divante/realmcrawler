using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Setup Checklist Validator
/// Press F1 in Unity Editor to validate all combat system components
/// Shows warnings/errors in console for missing configurations
/// </summary>
[RequireComponent(typeof(StateMachine))]
public class CombatSetupValidator : MonoBehaviour
{
    [Header("Core Components")]
    [SerializeField] private CombatInputHandler inputHandler;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private SpellManager spellManager;
    [SerializeField] private HitboxManager hitboxManager;
    [SerializeField] private StateMachine stateMachine;

    [Header("Debug Settings")]
    [SerializeField] private bool autoValidateOnStart = true;
    [SerializeField] private bool showValidationUI = false;

    private List<string> validationErrors = new();
    private List<string> validationWarnings = new();

    private void Start()
    {
        if (autoValidateOnStart)
        {
            ValidateSetup();
        }
    }

    private void Update()
    {
        // Press F1 to validate
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ValidateSetup();
        }
    }

    public void ValidateSetup()
    {
        validationErrors.Clear();
        validationWarnings.Clear();

        Debug.Log("\n========== COMBAT SYSTEM SETUP VALIDATION ==========\n");

        // 1. Validate Core Components
        ValidateCoreComponents();

        // 2. Validate Input System
        ValidateInputSystem();

        // 3. Validate Weapon System
        ValidateWeaponSystem();

        // 4. Validate Spell System
        ValidateSpellSystem();

        // 5. Validate State Machine
        ValidateStateMachine();

        // 6. Validate Scene Setup
        ValidateSceneSetup();

        // Print Results
        PrintResults();
    }

    #region Validation Methods

    private void ValidateCoreComponents()
    {
        Debug.Log("[1/6] Validating Core Components...");

        if (inputHandler == null)
        {
            validationErrors.Add("❌ CombatInputHandler not assigned!");
            if (TryGetComponent(out CombatInputHandler handler))
            {
                inputHandler = handler;
                validationWarnings.Add("⚠️ CombatInputHandler auto-assigned from GameObject");
            }
        }
        else if (!inputHandler.enabled)
        {
            validationWarnings.Add("⚠️ CombatInputHandler is disabled");
        }

        if (weaponManager == null)
        {
            validationErrors.Add("❌ WeaponManager not assigned!");
            if (TryGetComponent(out WeaponManager manager))
            {
                weaponManager = manager;
                validationWarnings.Add("⚠️ WeaponManager auto-assigned from GameObject");
            }
        }
        else if (!weaponManager.enabled)
        {
            validationWarnings.Add("⚠️ WeaponManager is disabled");
        }

        if (spellManager == null)
        {
            validationErrors.Add("❌ SpellManager not assigned!");
            if (TryGetComponent(out SpellManager manager))
            {
                spellManager = manager;
                validationWarnings.Add("⚠️ SpellManager auto-assigned from GameObject");
            }
        }
        else if (!spellManager.enabled)
        {
            validationWarnings.Add("⚠️ SpellManager is disabled");
        }

        if (hitboxManager == null)
        {
            validationErrors.Add("❌ HitboxManager not assigned!");
            if (TryGetComponent(out HitboxManager manager))
            {
                hitboxManager = manager;
                validationWarnings.Add("⚠️ HitboxManager auto-assigned from GameObject");
            }
        }
        else if (!hitboxManager.enabled)
        {
            validationWarnings.Add("⚠️ HitboxManager is disabled");
        }

        if (stateMachine == null)
        {
            validationErrors.Add("❌ StateMachine not assigned!");
            if (TryGetComponent(out StateMachine machine))
            {
                stateMachine = machine;
                validationWarnings.Add("⚠️ StateMachine auto-assigned from GameObject");
            }
        }
        else if (!stateMachine.enabled)
        {
            validationWarnings.Add("⚠️ StateMachine is disabled");
        }

        Debug.Log("  ✓ Core Components check complete\n");
    }

    private void ValidateInputSystem()
    {
        Debug.Log("[2/6] Validating Input System...");

        if (inputHandler == null)
        {
            validationErrors.Add("❌ Cannot validate InputSystem - CombatInputHandler missing");
            return;
        }

        // Check if InputActionAsset is assigned (field is private, so we check via reflection or assume it's set)
        // For now, we'll just check if the component exists and is enabled
        Debug.Log("  ✓ InputSystem check complete (manual verification needed in Inspector)\n");
        validationWarnings.Add("⚠️ Manually verify InputActionAsset is assigned in CombatInputHandler Inspector");
    }

    private void ValidateWeaponSystem()
    {
        Debug.Log("[3/6] Validating Weapon System...");

        if (weaponManager == null)
        {
            validationErrors.Add("❌ Cannot validate WeaponSystem - WeaponManager missing");
            return;
        }

        // Check if equipped weapon is assigned (field is private)
        validationWarnings.Add("⚠️ Manually verify equippedWeapon is assigned in WeaponManager Inspector");
        validationWarnings.Add("⚠️ Manually verify weaponSlots array is populated in WeaponManager Inspector");

        Debug.Log("  ✓ WeaponSystem check complete (manual verification needed in Inspector)\n");
    }

    private void ValidateSpellSystem()
    {
        Debug.Log("[4/6] Validating Spell System...");

        if (spellManager == null)
        {
            validationErrors.Add("❌ Cannot validate SpellSystem - SpellManager missing");
            return;
        }

        validationWarnings.Add("⚠️ Manually verify spellSlots[0-3] are assigned in SpellManager Inspector");

        Debug.Log("  ✓ SpellSystem check complete (manual verification needed in Inspector)\n");
    }

    private void ValidateStateMachine()
    {
        Debug.Log("[5/6] Validating State Machine...");

        if (stateMachine == null)
        {
            validationErrors.Add("❌ Cannot validate StateMachine - component missing");
            return;
        }

        validationWarnings.Add("⚠️ Manually verify startingState is assigned in StateMachine Inspector");
        validationWarnings.Add("⚠️ Manually verify CombatState asset exists and has actions configured");

        Debug.Log("  ✓ StateMachine check complete (manual verification needed in Inspector)\n");
    }

    private void ValidateSceneSetup()
    {
        Debug.Log("[6/6] Validating Scene Setup...");

        // Check for Animator
        if (!TryGetComponent<Animator>(out Animator animator))
        {
            validationWarnings.Add("⚠️ No Animator component found - animations won't play");
        }
        else
        {
            if (animator.runtimeAnimatorController == null)
            {
                validationWarnings.Add("⚠️ Animator has no Controller assigned");
            }
        }

        // Check for Collider2D (for hitbox parent)
        if (!TryGetComponent<Collider2D>(out Collider2D collider))
        {
            validationWarnings.Add("⚠️ No Collider2D found - may need for hitbox parent");
        }

        // Check for Rigidbody2D (for physics)
        if (!TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            validationWarnings.Add("⚠️ No Rigidbody2D found - physics interactions may not work");
        }

        Debug.Log("  ✓ Scene Setup check complete\n");
    }

    #endregion

    #region Results

    private void PrintResults()
    {
        Debug.Log("\n========== VALIDATION RESULTS ==========\n");

        if (validationErrors.Count > 0)
        {
            Debug.LogWarning("\n❌ ERRORS (" + validationErrors.Count + "):");
            foreach (var error in validationErrors)
            {
                Debug.LogWarning("  " + error);
            }
        }
        else
        {
            Debug.Log("\n✅ No errors found!");
        }

        if (validationWarnings.Count > 0)
        {
            Debug.LogWarning("\n⚠️ WARNINGS (" + validationWarnings.Count + "):");
            foreach (var warning in validationWarnings)
            {
                Debug.LogWarning("  " + warning);
            }
        }
        else
        {
            Debug.Log("\n✅ No warnings!");
        }

        Debug.Log("\n==========================================\n");

        if (validationErrors.Count == 0 && validationWarnings.Count == 0)
        {
            Debug.Log("🎉 COMBAT SYSTEM IS FULLY CONFIGURED! Ready to test!");
        }
        else
        {
            Debug.Log("🔧 Please fix the errors and warnings above before testing.");
        }
    }

    #endregion

    #region Editor Helpers

#if UNITY_EDITOR
    [ContextMenu("Validate Combat Setup")]
    public void ValidateFromContextMenu()
    {
        ValidateSetup();
    }
#endif

    #endregion
}

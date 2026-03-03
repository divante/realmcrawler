using System;
using RealmCrawler.Equipment;
using UnityEngine;

public enum AttackType { Light, Heavy }

[Serializable]
public class ComboAttackAction : ActionRuntime
{
    [SerializeField] private AttackType startOn = AttackType.Light;

    private CombatLoadout _loadout;
    private CombatContext _context;
    private CharacterAnimationController _anim;
    private AnimationEventBridge _bridge;
    private ICharacterController _controller;

    private ComboNode _currentNode;
    private bool _inputBuffered;
    private AttackType _bufferedType;
    private bool _windowOpen;

    public override void Initialize(ActionDefinition definition, GameObject owner)
    {
        base.Initialize(definition, owner);
        _loadout    = owner.GetComponent<CombatLoadout>();
        _context    = owner.GetComponent<CombatContext>();
        _anim       = owner.GetComponentInChildren<CharacterAnimationController>();
        _bridge     = owner.GetComponentInChildren<AnimationEventBridge>();
        _controller = owner.GetComponent<ICharacterController>();
    }

    public override void Activate()
    {
        _currentNode = startOn == AttackType.Light
            ? _loadout.EquippedWeapon?.LightComboRoot
            : _loadout.EquippedWeapon?.HeavyComboRoot;

        if (_currentNode == null)
        {
            ActionCompleted?.Invoke();
            return;
        }

        _inputBuffered = false;
        _windowOpen    = false;

        _context.BeginCombo(_currentNode);

        _bridge.OnComboWindowOpen  += OnComboWindowOpen;
        _bridge.OnComboWindowClose += OnComboWindowClose;
        _controller.AttackLightEvent += OnLightBuffered;
        _controller.AttackHeavyEvent += OnHeavyBuffered;

        FireCurrentNode();
    }

    public override void Deactivate()
    {
        _bridge.OnComboWindowOpen  -= OnComboWindowOpen;
        _bridge.OnComboWindowClose -= OnComboWindowClose;
        _controller.AttackLightEvent -= OnLightBuffered;
        _controller.AttackHeavyEvent -= OnHeavyBuffered;

        _context.EndCombo();
    }

    // Called by animation event via AnimationEventBridge
    private void OnComboWindowOpen()
    {
        _windowOpen    = true;
        _inputBuffered = false;
    }

    private void OnComboWindowClose()
    {
        _windowOpen = false;

        if (!_inputBuffered)
        {
            EndCombo();
            return;
        }

        ComboNode next = _bufferedType == AttackType.Light
            ? _currentNode.lightNext
            : _currentNode.heavyNext;

        if (next == null)
        {
            EndCombo();
            return;
        }

        _context.AdvanceCombo(next);
        _currentNode   = next;
        _inputBuffered = false;
        FireCurrentNode();
    }

    private void OnLightBuffered()
    {
        if (!_windowOpen) return;
        _inputBuffered = true;
        _bufferedType  = AttackType.Light;
    }

    private void OnHeavyBuffered()
    {
        if (!_windowOpen) return;
        _inputBuffered = true;
        _bufferedType  = AttackType.Heavy;
    }

    private void FireCurrentNode()
    {
        if (_currentNode?.hit == null) { EndCombo(); return; }

        int trigger = startOn == AttackType.Light
            ? CharacterAnimationController.AnimTriggerLightAttack
            : CharacterAnimationController.AnimTriggerHeavyAttack;

        _anim.TriggerCombat(trigger);

        // TODO: spawn _currentNode.hit.ProjectilePrefab / apply damage
    }

    private void EndCombo()
    {
        _context.EndCombo();
        ActionCompleted?.Invoke();
    }
}

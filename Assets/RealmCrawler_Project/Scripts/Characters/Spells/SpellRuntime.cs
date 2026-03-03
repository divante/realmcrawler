using System;
using UnityEngine;

namespace RealmCrawler.Characters.Spells
{
    [Serializable]
    public class SpellRuntime : ActionRuntime
    {
        protected SpellDefinition _spellDefinition;
        protected int _spellSlotIndex;
        protected float _cooldownTimer;
        protected bool _isOnCooldown;
        protected bool _isCasting;
        protected float _castTimer;

        public int SpellSlotIndex => _spellSlotIndex;
        public bool IsOnCooldown => _isOnCooldown;
        public float CooldownRemaining => _cooldownTimer;
        public bool IsCasting => _isCasting;

        public override void Initialize(ActionDefinition definition, GameObject owner)
        {
            base.Initialize(definition, owner);
            _spellDefinition = definition as SpellDefinition;
            if (_spellDefinition == null)
            {
                Debug.LogError("[SpellRuntime] ActionDefinition is not a SpellDefinition!");
            }
        }

        public void SetSpellSlotIndex(int index)
        {
            _spellSlotIndex = index;
        }

        public override void Activate()
        {
            base.Activate();
            _isCasting = false;
            _castTimer = 0f;
            _cooldownTimer = 0f;
            _isOnCooldown = false;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _isCasting = false;
        }

        public override void Update()
        {
            base.Update();

            // Handle cooldown
            if (_isOnCooldown)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0f)
                {
                    _isOnCooldown = false;
                    Debug.Log($"[SpellRuntime] Spell {_definition.actionName} cooldown complete");
                }
            }

            // Handle casting
            if (_isCasting && _spellDefinition != null)
            {
                _castTimer += Time.deltaTime;
                if (_castTimer >= _spellDefinition.castTime)
                {
                    ExecuteSpell();
                    _isCasting = false;
                    _castTimer = 0f;
                    StartCooldown();
                }
            }
        }

        public bool CanCast()
        {
            return !_isOnCooldown && !_isCasting;
        }

        public void StartCast()
        {
            if (!CanCast())
            {
                Debug.LogWarning($"[SpellRuntime] Cannot cast {_definition.actionName} - on cooldown or already casting");
                return;
            }

            _isCasting = true;
            _castTimer = 0f;
            Debug.Log($"[SpellRuntime] Starting cast for {_definition.actionName}");
        }

        protected virtual void ExecuteSpell()
        {
            Debug.Log($"[SpellRuntime] Executing spell: {_definition.actionName}");
            // Override in specific spell types
        }

        protected virtual void StartCooldown()
        {
            if (_spellDefinition != null)
            {
                _cooldownTimer = _spellDefinition.cooldown;
                _isOnCooldown = true;
                Debug.Log($"[SpellRuntime] Started cooldown for {_definition.actionName}: {_spellDefinition.cooldown}s");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}

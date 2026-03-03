using System;
using RealmCrawler.Spells;
using UnityEngine;

[Serializable]
public class SpellCastAction : ActionRuntime
{
    [SerializeField] private int slot = 0;
    [SerializeField] private float midComboDamageBonus = 1.5f;

    private CombatLoadout _loadout;
    private CombatContext _context;
    private VeilSystem _veil;
    private CharacterAnimationController _anim;
    private AnimationEventBridge _bridge;
    private ICharacterController _controller;
    private Transform __firePoint;

    private SpellData _spell;
    private bool _active;

    // Channeled/Charged state
    private float _chargeTimer;
    private float _channelTimer;
    private float _channelFireTimer;

    public override void Initialize(ActionDefinition definition, GameObject owner)
    {
        base.Initialize(definition, owner);
        _loadout    = owner.GetComponent<CombatLoadout>();
        _context    = owner.GetComponent<CombatContext>();
        _veil       = owner.GetComponent<VeilSystem>();
        _anim       = owner.GetComponentInChildren<CharacterAnimationController>();
        _bridge     = owner.GetComponentInChildren<AnimationEventBridge>();
        _controller = owner.GetComponent<ICharacterController>();

        // Resolve fire point from a tagged child, fall back to owner root
        GameObject fp = GameObject.FindGameObjectWithTag("FirePoint");
        __firePoint = (fp != null && fp.transform.IsChildOf(owner.transform))
            ? fp.transform
            : owner.transform;
    }

    public override void Activate()
    {
        _spell = _loadout.GetSpell(slot);
        if (_spell == null) { ActionCompleted?.Invoke(); return; }

        _active = true;
        _anim.TriggerCombat(CharacterAnimationController.AnimTriggerSpellCast);

        switch (_spell.CastType)
        {
            case SpellCastType.Instant:
                FireSpell();
                _bridge.OnAnimationFinished += OnAnimationFinished;
                break;

            case SpellCastType.Charged:
                _chargeTimer = 0f;
                _controller.SpellCastReleasedEvent += OnSpellReleased;
                break;

            case SpellCastType.Channeled:
                _channelTimer    = 0f;
                _channelFireTimer = 0f;
                _controller.SpellCastReleasedEvent += OnSpellReleased;
                break;
        }
    }

    public override void Update()
    {
        if (!_active) return;

        switch (_spell.CastType)
        {
            case SpellCastType.Charged:
                _chargeTimer = Mathf.Min(_chargeTimer + Time.deltaTime, _spell.MaxChargeTime);
                break;

            case SpellCastType.Channeled:
                _channelTimer    += Time.deltaTime;
                _channelFireTimer += Time.deltaTime;

                if (_channelFireTimer >= _spell.ChannelFireRate)
                {
                    _channelFireTimer = 0f;
                    FireSpell();
                }

                if (_channelTimer >= _spell.MaxChannelDuration)
                    FinishCast();
                break;
        }
    }

    public override void Deactivate()
    {
        _active = false;
        _bridge.OnAnimationFinished      -= OnAnimationFinished;
        _controller.SpellCastReleasedEvent -= OnSpellReleased;
    }

    private void OnSpellReleased(int releasedSlot)
    {
        if (releasedSlot != slot) return;
        if (_spell.CastType == SpellCastType.Charged)
        {
            float chargeRatio = _chargeTimer / _spell.MaxChargeTime;
            FireSpell(chargeRatio);
        }
        FinishCast();
    }

    private void OnAnimationFinished() => FinishCast();

    private void FireSpell(float chargeRatio = 1f)
    {
        if (_spell.SpellPrefab == null) return;

        float damage = _spell.BaseDamage * chargeRatio;

        // Mid-combo bonus
        if (_context.IsInCombo)
            damage *= midComboDamageBonus;

        // Veil effects
        damage *= _veil.SpellPowerMultiplier;

        Quaternion fireRotation = _firePoint.rotation;
        if (_veil.RollMisfire())
            fireRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);

        GameObject instance = GameObject.Instantiate(_spell.SpellPrefab, _firePoint.position, fireRotation);

        // TODO: pass damage to the projectile component on instance

        SpellMutation mutation = _veil.SelectMutation(_spell);
        mutation?.Apply(instance, _firePoint.position, fireRotation);

        _veil.ConsumeVeil(_spell.VeilCost);
    }

    private void FinishCast()
    {
        _active = false;
        _controller.SpellCastReleasedEvent -= OnSpellReleased;
        _bridge.OnAnimationFinished        -= OnAnimationFinished;
        ActionCompleted?.Invoke();
    }
}

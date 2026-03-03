using System;
using UnityEngine;

/// <summary>
/// Runtime execution of weapon attacks with phases and hitboxes
/// </summary>
[Serializable]
public class WeaponAttackRuntime : ActionRuntime
{
    public enum AttackType { Fast, Heavy }
    
    [SerializeField] private WeaponDefinition _weapon;
    [SerializeField] private AttackType _attackType;
    [SerializeField] private AttackPattern _currentPattern;
    
    private Transform _hitboxTransform;
    private Collider2D _hitboxCollider;
    private LayerMask _enemyLayerMask;
    
    public WeaponDefinition Weapon => _weapon;
    public AttackType AttackType => _attackType;
    public bool CanChainNextAttack => _currentPattern?.canChain ?? false;
    
    public override void Initialize(ActionDefinition definition, GameObject owner)
    {
        base.Initialize(definition, owner);
        
        if (definition is WeaponAttackDefinition weaponDef)
        {
            _weapon = weaponDef.Weapon;
            _attackType = weaponDef.AttackType;
            _currentPattern = _attackType == AttackType.Fast 
                ? _weapon.fastAttack 
                : _weapon.heavyAttack;
            
            _enemyLayerMask = LayerMask.GetMask("Enemy");
        }
        else
        {
            Debug.LogError($"[WeaponAttackRuntime] Invalid ActionDefinition type: {definition.GetType()}");
        }
    }
    
    public override void Activate()
    {
        base.Activate();
        Debug.Log($"[WeaponAttackRuntime] Activated {_definition.actionName} ({_attackType})");
        
        // Start with Windup phase
        CurrentPhase = ActionPhase.Windup;
        PhaseTimer = _currentPattern.windupTime;
    }
    
    public override void Deactivate()
    {
        base.Deactivate();
        DestroyHitbox();
        Debug.Log($"[WeaponAttackRuntime] Deactivated {_definition.actionName}");
    }
    
    public override void Update()
    {
        if (CurrentPhase == ActionPhase.None) return;
        
        PhaseTimer -= Time.deltaTime;
        
        switch (CurrentPhase)
        {
            case ActionPhase.Windup:
                if (PhaseTimer <= 0)
                {
                    CurrentPhase = ActionPhase.Execute;
                    PhaseTimer = _currentPattern.executeTime;
                    SpawnHitbox();
                    OnPhaseChanged?.Invoke(CurrentPhase);
                }
                break;
                
            case ActionPhase.Execute:
                if (PhaseTimer <= 0)
                {
                    CurrentPhase = ActionPhase.Recovery;
                    PhaseTimer = _currentPattern.recoveryTime;
                    DestroyHitbox();
                    OnPhaseChanged?.Invoke(CurrentPhase);
                }
                break;
                
            case ActionPhase.Recovery:
                if (PhaseTimer <= 0)
                {
                    CurrentPhase = ActionPhase.Completed;
                    OnActionCompleted?.Invoke();
                }
                break;
        }
    }
    
    public override void FixedUpdate()
    {
        // Hit detection happens in FixedUpdate for physics consistency
        if (CurrentPhase == ActionPhase.Execute && _hitboxCollider != null)
        {
            CheckHitboxCollisions();
        }
    }
    
    private void SpawnHitbox()
    {
        // Create hitbox at weapon tip position
        GameObject hitboxObj = new GameObject($"{ _definition.actionName}_Hitbox");
        hitboxObj.transform.SetParent(_owner.transform);
        
        // Position at weapon tip (you'll need to adjust this based on your weapon model)
        hitboxObj.transform.localPosition = new Vector3(0.5f, 0, 0); // Adjust based on weapon
        hitboxObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        
        _hitboxTransform = hitboxObj.transform;
        
        // Create box collider for hit detection
        _hitboxCollider = hitboxObj.AddComponent<BoxCollider2D>();
        _hitboxCollider.size = new Vector2(_currentPattern.hitboxRange, _currentPattern.hitboxHeight);
        _hitboxCollider.offset = new Vector2(_currentPattern.hitboxRange / 2, 0);
        _hitboxCollider.isTrigger = true;
        
        // Set layer for collision filtering
        hitboxObj.layer = LayerMask.NameToLayer("Combat");
        
        // Rotate hitbox with weapon (if needed)
        _hitboxTransform.localRotation = Quaternion.Euler(0, 0, _currentPattern.hitboxAngle);
    }
    
    private void DestroyHitbox()
    {
        if (_hitboxCollider != null)
        {
            GameObject.Destroy(_hitboxCollider.gameObject);
            _hitboxCollider = null;
            _hitboxTransform = null;
        }
    }
    
    private void CheckHitboxCollisions()
    {
        Collider2D[] colliders = Physics2D.OverlapCollider(_hitboxCollider, _enemyLayerMask);
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                ApplyDamage(collider.gameObject);
            }
        }
    }
    
    public void ApplyDamage(GameObject target)
    {
        // TODO: Integrate with damage system
        Debug.Log($"[WeaponAttackRuntime] Dealt {_currentPattern.damage} damage to {target.name}");
        
        // Trigger damage events
        OnDamageApplied?.Invoke(target, _currentPattern.damage);
    }
    
    // Events for combat feedback
    public event Action<GameObject, float> OnDamageApplied;
}

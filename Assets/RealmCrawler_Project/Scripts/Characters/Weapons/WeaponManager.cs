using UnityEngine;

/// <summary>
/// Manages equipped weapon and weapon switching
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField] private WeaponDefinition _equippedWeapon;
    
    [Header("Available Weapons")]
    [SerializeField] private WeaponDefinition[] _weaponSlots;
    [SerializeField] private int _currentWeaponIndex = 0;
    
    [Header("Settings")]
    [SerializeField] private float _switchDuration = 0.5f;
    
    // Events
    public event System.Action<WeaponDefinition> OnWeaponEquipped;
    public event System.Action<int> OnWeaponSwitched;
    
    // State
    private bool _isSwitching = false;
    
    // Properties
    public WeaponDefinition EquippedWeapon => _equippedWeapon;
    public int CurrentWeaponIndex => _currentWeaponIndex;
    public bool IsSwitching => _isSwitching;
    
    private void Awake()
    {
        InitializeWeapons();
    }
    
    private void InitializeWeapons()
    {
        if (_weaponSlots.Length > 0 && _equippedWeapon == null)
        {
            EquipWeapon(0);
        }
    }
    
    /// <summary>
    /// Equip weapon by index
    /// </summary>
    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= _weaponSlots.Length)
        {
            Debug.LogWarning($"[WeaponManager] Invalid weapon index: {index}");
            return;
        }
        
        if (_isSwitching)
        {
            Debug.LogWarning("[WeaponManager] Already switching weapons!");
            return;
        }
        
        if (_currentWeaponIndex == index)
        {
            return; // Already equipped
        }
        
        _currentWeaponIndex = index;
        _equippedWeapon = _weaponSlots[index];
        
        OnWeaponEquipped?.Invoke(_equippedWeapon);
        OnWeaponSwitched?.Invoke(_currentWeaponIndex);
        
        Debug.Log($"[WeaponManager] Equipped: {_equippedWeapon.weaponName}");
    }
    
    /// <summary>
    /// Switch to next weapon
    /// </summary>
    public void NextWeapon()
    {
        if (_weaponSlots.Length == 0) return;
        
        int nextIndex = (_currentWeaponIndex + 1) % _weaponSlots.Length;
        EquipWeapon(nextIndex);
    }
    
    /// <summary>
    /// Switch to previous weapon
    /// </summary>
    public void PreviousWeapon()
    {
        if (_weaponSlots.Length == 0) return;
        
        int prevIndex = (_currentWeaponIndex - 1 + _weaponSlots.Length) % _weaponSlots.Length;
        EquipWeapon(prevIndex);
    }
    
    /// <summary>
    /// Check if can switch weapons (not during action recovery)
    /// </summary>
    public bool CanSwitchWeapons()
    {
        return !_isSwitching;
    }
    
    /// <summary>
    /// Get attack pattern for fast attack
    /// </summary>
    public AttackPattern GetFastAttackPattern()
    {
        return _equippedWeapon?.fastAttack;
    }
    
    /// <summary>
    /// Get attack pattern for heavy attack
    /// </summary>
    public AttackPattern GetHeavyAttackPattern()
    {
        return _equippedWeapon?.heavyAttack;
    }
}

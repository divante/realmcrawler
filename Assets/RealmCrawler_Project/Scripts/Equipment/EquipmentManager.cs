using UnityEngine;
using UnityEngine.Events;
using RealmCrawler.Equipment;
using RealmCrawler.Player;

public class EquipmentManager : MonoBehaviour
{
    [Header("Current Equipment Loadout")]
    [SerializeField] private HatData equippedHat;
    [SerializeField] private CloakData equippedCloak;
    [SerializeField] private BootsData equippedBoots;
    [SerializeField] private WeaponData equippedWeapon;
    [SerializeField] private ReliquaryData equippedReliquary;

    [Header("Base Stats")]
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseMana = 100f;
    [SerializeField] private float baseMoveSpeed = 10f;
    [SerializeField] private float baseXpRadius = 5f;

    [Header("References")]
    [SerializeField] private PlayerVisualEquipment visualEquipment;
    [SerializeField] private CharacterPhysics characterPhysics;
    [SerializeField] private SpellManager spellManager;

    private float currentMaxHealth;
    private float currentMaxMana;
    private float currentMoveSpeed;
    private float currentXpRadius;

    public float MaxHealth => currentMaxHealth;
    public float MaxMana => currentMaxMana;
    public float MoveSpeed => currentMoveSpeed;
    public float XpRadius => currentXpRadius;

    public HatData EquippedHat => equippedHat;
    public CloakData EquippedCloak => equippedCloak;
    public BootsData EquippedBoots => equippedBoots;
    public WeaponData EquippedWeapon => equippedWeapon;
    public ReliquaryData EquippedReliquary => equippedReliquary;

    public UnityEvent<HatData> OnHatEquipped;
    public UnityEvent<CloakData> OnCloakEquipped;
    public UnityEvent<BootsData> OnBootsEquipped;
    public UnityEvent<WeaponData> OnWeaponEquipped;
    public UnityEvent<ReliquaryData> OnReliquaryEquipped;

    void Awake()
    {
        if (visualEquipment == null)
            visualEquipment = GetComponent<PlayerVisualEquipment>();

        if (characterPhysics == null)
            characterPhysics = GetComponent<CharacterPhysics>();

        if (spellManager == null)
            spellManager = GetComponent<SpellManager>();
    }

    void Start()
    {
        ApplyEquipmentLoadout();
    }

    public void ApplyEquipmentLoadout()
    {
        CalculateStats();
        ApplyVisuals();
        ApplyWeapon();
        ApplyMovementSpeed();
    }

    private void CalculateStats()
    {
        currentMaxHealth = baseHealth;
        currentMaxMana = baseMana;
        currentMoveSpeed = baseMoveSpeed;
        currentXpRadius = baseXpRadius;

        // NOTE: Old direct stat bonus system removed - use StatModifierBase system instead
        // Equipment now applies bonuses via ApplyModifiers(CharacterData) method
        // which uses the StatModifierBase system for all stat changes
    }

    private void ApplyVisuals()
    {
        if (visualEquipment == null)
            return;

        visualEquipment.EquipHat(equippedHat);
        visualEquipment.EquipCloak(equippedCloak);
        visualEquipment.EquipBoots(equippedBoots);
        visualEquipment.EquipReliquary(equippedReliquary);
        visualEquipment.EquipWeapon(equippedWeapon);
    }

    private void ApplyWeapon()
    {
        if (spellManager != null && equippedWeapon != null)
        {
            spellManager.EquipWeapon(equippedWeapon);
        }
    }

    private void ApplyMovementSpeed()
    {
        if (characterPhysics != null)
        {
            characterPhysics.SetMoveSpeed(currentMoveSpeed);
        }
    }

    public void SetEquipment(HatData hat, CloakData cloak, BootsData boots, WeaponData weapon, ReliquaryData reliquary)
    {
        equippedHat = hat;
        equippedCloak = cloak;
        equippedBoots = boots;
        equippedWeapon = weapon;
        equippedReliquary = reliquary;

        OnHatEquipped?.Invoke(hat);
        OnCloakEquipped?.Invoke(cloak);
        OnBootsEquipped?.Invoke(boots);
        OnWeaponEquipped?.Invoke(weapon);
        OnReliquaryEquipped?.Invoke(reliquary);
    }

    public void SetHat(HatData hat)
    {
        equippedHat = hat;
        OnHatEquipped?.Invoke(hat);
    }

    public void SetCloak(CloakData cloak)
    {
        equippedCloak = cloak;
        OnCloakEquipped?.Invoke(cloak);
    }

    public void SetBoots(BootsData boots)
    {
        equippedBoots = boots;
        OnBootsEquipped?.Invoke(boots);
    }

    public void SetWeapon(WeaponData weapon)
    {
        equippedWeapon = weapon;
        OnWeaponEquipped?.Invoke(weapon);
    }

    public void SetReliquary(ReliquaryData reliquary)
    {
        equippedReliquary = reliquary;
        OnReliquaryEquipped?.Invoke(reliquary);
    }
}

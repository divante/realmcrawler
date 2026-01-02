using System;
using System.Collections.Generic;
using UnityEngine;
using RealmCrawler.Spells;
using RealmCrawler.Equipment;
using RealmCrawler.Combat;

namespace RealmCrawler.Player
{
    public class SpellManager : MonoBehaviour
    {
        [Header("Spell Slots")]
        [SerializeField] private SpellData qSpellSlot;
        [SerializeField] private SpellData eSpellSlot;
        [SerializeField] private SpellData shiftSpellSlot;

        [Header("Weapon & Cantrips")]
        [SerializeField] private WeaponData equippedWeapon;

        [Header("Spell Cast Settings")]
        [SerializeField] private Transform spellCastPoint;
        [SerializeField] private float globalCooldownDuration = 0.1f;

        private Dictionary<SpellData, float> spellCooldowns = new Dictionary<SpellData, float>();
        private float globalCooldownTimer = 0f;
        private float currentMana;
        private float maxMana = 100f;

        public event Action<SpellData> OnSpellCast;
        public event Action<CantripData> OnCantripCast;
        public event Action<float, float> OnManaChanged;

        public SpellData QSpellSlot => qSpellSlot;
        public SpellData ESpellSlot => eSpellSlot;
        public SpellData ShiftSpellSlot => shiftSpellSlot;
        public CantripData PrimaryCantrip => equippedWeapon?.PrimaryCantrip;
        public CantripData SecondaryCantrip => equippedWeapon?.SecondaryCantrip;
        public float CurrentMana => currentMana;
        public float MaxMana => maxMana;

        private void Awake()
        {
            currentMana = maxMana;
        }

        private void Update()
        {
            UpdateCooldowns();
        }

        public void EquipWeapon(WeaponData weapon)
        {
            equippedWeapon = weapon;
        }

        public void AssignSpellToSlot(SpellData spell, int slotIndex)
        {
            switch (slotIndex)
            {
                case 0:
                    qSpellSlot = spell;
                    break;
                case 1:
                    eSpellSlot = spell;
                    break;
                case 2:
                    shiftSpellSlot = spell;
                    break;
                default:
                    Debug.LogWarning($"Invalid spell slot index: {slotIndex}");
                    break;
            }
        }

        public bool TryCastSpell(SpellData spell)
        {
            if (spell == null) return false;
            if (!CanCastSpell(spell)) return false;

            CastSpell(spell);
            return true;
        }

        public bool TryCastCantrip(CantripData cantrip)
        {
            if (cantrip == null) return false;
            if (globalCooldownTimer > 0f) return false;

            CastCantrip(cantrip);
            return true;
        }

        public bool CanCastSpell(SpellData spell)
        {
            if (spell == null) return false;
            if (globalCooldownTimer > 0f) return false;
            if (currentMana < spell.ManaCost) return false;
            if (IsSpellOnCooldown(spell)) return false;

            return true;
        }

        public bool IsSpellOnCooldown(SpellData spell)
        {
            if (spell == null) return false;
            return spellCooldowns.ContainsKey(spell) && spellCooldowns[spell] > 0f;
        }

        public float GetSpellCooldownRemaining(SpellData spell)
        {
            if (spell == null) return 0f;
            return spellCooldowns.ContainsKey(spell) ? spellCooldowns[spell] : 0f;
        }

        public void SetMaxMana(float value)
        {
            maxMana = value;
            currentMana = Mathf.Min(currentMana, maxMana);
            OnManaChanged?.Invoke(currentMana, maxMana);
        }

        public void AddMana(float amount)
        {
            currentMana = Mathf.Min(currentMana + amount, maxMana);
            OnManaChanged?.Invoke(currentMana, maxMana);
        }

        public void RegenerateMana(float amount)
        {
            AddMana(amount);
        }

        private void CastSpell(SpellData spell)
        {
            currentMana -= spell.ManaCost;
            OnManaChanged?.Invoke(currentMana, maxMana);

            if (!spellCooldowns.ContainsKey(spell))
            {
                spellCooldowns.Add(spell, spell.CooldownDuration);
            }
            else
            {
                spellCooldowns[spell] = spell.CooldownDuration;
            }

            globalCooldownTimer = globalCooldownDuration;

            if (spell.SpellPrefab != null && spellCastPoint != null)
            {
                Instantiate(spell.SpellPrefab, spellCastPoint.position, spellCastPoint.rotation);
            }

            if (spell.CastSound != null)
            {
                AudioSource.PlayClipAtPoint(spell.CastSound, transform.position);
            }

            OnSpellCast?.Invoke(spell);
        }

        private void CastCantrip(CantripData cantrip)
        {
            globalCooldownTimer = globalCooldownDuration;

            if (cantrip.ProjectilePrefab != null && spellCastPoint != null)
            {
                GameObject projectileObj = Instantiate(cantrip.ProjectilePrefab, spellCastPoint.position, spellCastPoint.rotation);
                
                ProjectileMover projectile = projectileObj.GetComponent<ProjectileMover>();
                if (projectile != null)
                {
                    float finalDamage = cantrip.BaseDamage;
                    if (equippedWeapon != null)
                    {
                        finalDamage *= equippedWeapon.DamageMultiplier;
                    }
                    
                    projectile.SetDamage(finalDamage);
                    projectile.SetSpeed(cantrip.ProjectileSpeed);
                }
            }

            if (cantrip.CastSound != null)
            {
                AudioSource.PlayClipAtPoint(cantrip.CastSound, transform.position);
            }

            OnCantripCast?.Invoke(cantrip);
        }

        private void UpdateCooldowns()
        {
            if (globalCooldownTimer > 0f)
            {
                globalCooldownTimer -= Time.deltaTime;
            }

            List<SpellData> keys = new List<SpellData>(spellCooldowns.Keys);
            foreach (SpellData spell in keys)
            {
                if (spellCooldowns[spell] > 0f)
                {
                    spellCooldowns[spell] -= Time.deltaTime;
                }
            }
        }
    }
}

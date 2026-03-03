using RealmCrawler.Equipment;
using RealmCrawler.Spells;
using UnityEngine;

/// <summary>
/// Holds the player's currently equipped weapon and spell slots.
/// Fed by EquipmentManager; queried by ComboAttackAction and SpellCastAction.
/// </summary>
public class CombatLoadout : MonoBehaviour
{
    [SerializeField] private WeaponData equippedWeapon;
    [SerializeField] private SpellData[] equippedSpells = new SpellData[4];

    public WeaponData EquippedWeapon => equippedWeapon;

    public SpellData GetSpell(int slot)
    {
        if (slot < 0 || slot >= equippedSpells.Length) return null;
        return equippedSpells[slot];
    }

    public void SetWeapon(WeaponData weapon) => equippedWeapon = weapon;

    public void SetSpell(int slot, SpellData spell)
    {
        if (slot < 0 || slot >= equippedSpells.Length) return;
        equippedSpells[slot] = spell;
    }
}

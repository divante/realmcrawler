using UnityEngine;
using RealmCrawler.Spells;

namespace RealmCrawler.Progression
{
    public enum UpgradeType
    {
        HealthBoost,
        ManaBoost,
        SpellUnlock,
        SpellDamageBoost,
        CantripDamageBoost,
        CurrencyBonus
    }

    [CreateAssetMenu(fileName = "New Upgrade", menuName = "RealmCrawler/Progression/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string upgradeName;
        [SerializeField] [TextArea(2, 3)] private string description;
        [SerializeField] private Sprite icon;

        [Header("Upgrade Properties")]
        [SerializeField] private UpgradeType upgradeType;
        [SerializeField] private float value;

        [Header("Spell Unlock (if applicable)")]
        [SerializeField] private SpellData spellToUnlock;

        [Header("Weighting")]
        [SerializeField] [Range(0f, 1f)] private float spawnWeight = 1f;

        public string UpgradeName => upgradeName;
        public string Description => description;
        public Sprite Icon => icon;
        public UpgradeType Type => upgradeType;
        public float Value => value;
        public SpellData SpellToUnlock => spellToUnlock;
        public float SpawnWeight => spawnWeight;
    }
}

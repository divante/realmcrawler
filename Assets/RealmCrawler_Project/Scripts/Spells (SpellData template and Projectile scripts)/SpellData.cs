using UnityEngine;

namespace RealmCrawler.Spells
{
    public enum SpellElement
    {
        Eldritch,
        Fire,
        Lightning
    }

    public enum SpellType
    {
        Projectile,
        AOE,
        Debuff,
        Utility
    }

    [CreateAssetMenu(fileName = "New Spell", menuName = "RealmCrawler/Spells/Spell Data")]
    public class SpellData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string spellName;
        [SerializeField] [TextArea(2, 4)] private string description;
        [SerializeField] private Sprite icon;

        [Header("Spell Properties")]
        [SerializeField] private SpellElement element;
        [SerializeField] private SpellType type;

        [Header("Resource Costs")]
        [SerializeField] private float manaCost = 10f;
        [SerializeField] private float cooldownDuration = 2f;

        [Header("Damage & Effects")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float aoeRadius = 0f;
        [SerializeField] private float debuffDuration = 0f;

        [Header("Visuals & Audio")]
        [SerializeField] private GameObject spellPrefab;
        [SerializeField] private AudioClip castSound;

        public string SpellName => spellName;
        public string Description => description;
        public Sprite Icon => icon;
        public SpellElement Element => element;
        public SpellType Type => type;
        public float ManaCost => manaCost;
        public float CooldownDuration => cooldownDuration;
        public float BaseDamage => baseDamage;
        public float AoeRadius => aoeRadius;
        public float DebuffDuration => debuffDuration;
        public GameObject SpellPrefab => spellPrefab;
        public AudioClip CastSound => castSound;
    }
}

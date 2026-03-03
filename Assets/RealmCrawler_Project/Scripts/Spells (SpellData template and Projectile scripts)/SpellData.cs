using System.Collections.Generic;
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

    public enum SpellCastType
    {
        Instant,
        Charged,
        Channeled
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

        [Header("Cast Type")]
        [SerializeField] private SpellCastType castType = SpellCastType.Instant;
        [SerializeField] private float maxChargeTime = 1.5f;
        [SerializeField] private float chargeMultiplier = 2f;
        [SerializeField] private float channelFireRate = 0.2f;
        [SerializeField] private float maxChannelDuration = 3f;

        [Header("Veil Cost")]
        [SerializeField] private float veilCost = 0.05f;

        [Header("Damage & Effects")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float aoeRadius = 0f;
        [SerializeField] private float debuffDuration = 0f;

        [Header("Instability Mutations")]
        [SerializeField] private List<SpellMutation> possibleMutations = new();

        [Header("Visuals & Audio")]
        [SerializeField] private GameObject spellPrefab;
        [SerializeField] private AudioClip castSound;

        public string SpellName => spellName;
        public string Description => description;
        public Sprite Icon => icon;
        public SpellElement Element => element;
        public SpellType Type => type;
        public SpellCastType CastType => castType;
        public float MaxChargeTime => maxChargeTime;
        public float ChargeMultiplier => chargeMultiplier;
        public float ChannelFireRate => channelFireRate;
        public float MaxChannelDuration => maxChannelDuration;
        public float VeilCost => veilCost;
        public float BaseDamage => baseDamage;
        public float AoeRadius => aoeRadius;
        public float DebuffDuration => debuffDuration;
        public IReadOnlyList<SpellMutation> PossibleMutations => possibleMutations;
        public GameObject SpellPrefab => spellPrefab;
        public AudioClip CastSound => castSound;
    }
}

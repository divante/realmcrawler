using UnityEngine;

namespace RealmCrawler.Spells
{
    [CreateAssetMenu(fileName = "New Cantrip", menuName = "RealmCrawler/Spells/Cantrip Data")]
    public class CantripData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string cantripName;
        [SerializeField] [TextArea(2, 4)] private string description;
        [SerializeField] private Sprite icon;

        [Header("Combat Properties")]
        [SerializeField] private float baseDamage = 5f;
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private float projectileSpeed = 10f;

        [Header("Visuals & Audio")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private AudioClip castSound;

        public string CantripName => cantripName;
        public string Description => description;
        public Sprite Icon => icon;
        public float BaseDamage => baseDamage;
        public float AttackSpeed => attackSpeed;
        public float ProjectileSpeed => projectileSpeed;
        public GameObject ProjectilePrefab => projectilePrefab;
        public AudioClip CastSound => castSound;
    }
}

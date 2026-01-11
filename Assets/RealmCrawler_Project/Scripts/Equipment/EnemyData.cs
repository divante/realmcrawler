using UnityEngine;

namespace RealmCrawler.Enemies
{
    public enum EnemyBehaviorType
    {
        MeleeRusher,
        RangedShooter,
        Swarmer,
        Tank,
        Support
    }

    [CreateAssetMenu(fileName = "New Enemy", menuName = "RealmCrawler/Enemies/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string enemyName;
        [SerializeField] private EnemyBehaviorType behaviorType;

        [Header("Base Stats")]
        [SerializeField] private float baseHealth = 50f;
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float baseSpeed = 3.5f;

        [Header("XP Rewards")]
        [SerializeField] private int essenceDropCount = 1;

        [Header("Prefabs")]
        [SerializeField] private GameObject enemyPrefab;

        public string EnemyName => enemyName;
        public EnemyBehaviorType BehaviorType => behaviorType;
        public float BaseHealth => baseHealth;
        public float BaseDamage => baseDamage;
        public float BaseSpeed => baseSpeed;
        public int EssenceDropCount => essenceDropCount;
        public GameObject EnemyPrefab => enemyPrefab;
    }
}

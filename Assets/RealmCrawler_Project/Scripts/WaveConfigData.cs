using UnityEngine;
using RealmCrawler.Enemies;

namespace RealmCrawler.Waves
{
    [System.Serializable]
    public class WavePhaseConfig
    {
        [Header("Wave Range")]
        public int startWave = 1;
        public int endWave = 2;

        [Header("Enemy Composition (Percentages)")]
        [Range(0f, 1f)] public float skeletonPercent = 1f;
        [Range(0f, 1f)] public float rangedImpPercent = 0f;
        [Range(0f, 1f)] public float meleeImpPercent = 0f;
        [Range(0f, 1f)] public float golemPercent = 0f;
        [Range(0f, 1f)] public float necromongerPercent = 0f;

        public bool IsValidForWave(int waveNumber)
        {
            return waveNumber >= startWave && waveNumber <= endWave;
        }

        public float GetTotalPercent()
        {
            return skeletonPercent + rangedImpPercent + meleeImpPercent + golemPercent + necromongerPercent;
        }
    }

    [CreateAssetMenu(fileName = "Wave Config", menuName = "RealmCrawler/Waves/Wave Config Data")]
    public class WaveConfigData : ScriptableObject
    {
        [Header("Base Wave Settings")]
        [SerializeField] private int baseEnemyCount = 10;
        [SerializeField] [Range(0f, 1f)] private float enemyCountScalingPercent = 0.1f;

        [Header("Stat Scaling Per Wave")]
        [SerializeField] [Range(0f, 1f)] private float healthScalingPercent = 0.15f;
        [SerializeField] [Range(0f, 1f)] private float damageScalingPercent = 0.1f;
        [SerializeField] [Range(0f, 1f)] private float speedScalingPercent = 0.05f;

        [Header("Spawn Timing")]
        [SerializeField] private float initialSpawnPercent = 0.5f;
        [SerializeField] private float trickleSpawnPercent = 0.1f;
        [SerializeField] private float spawnIntervalMin = 2f;
        [SerializeField] private float spawnIntervalMax = 3f;

        [Header("Wave Transitions")]
        [SerializeField] private float waveBreakerDuration = 10f;

        [Header("Spawn Points")]
        [SerializeField] private int minSpawnPoints = 4;
        [SerializeField] private int maxSpawnPoints = 8;

        [Header("Wave Phases")]
        [SerializeField] private WavePhaseConfig[] wavePhases = new WavePhaseConfig[]
        {
            new WavePhaseConfig { startWave = 1, endWave = 2, skeletonPercent = 1f },
            new WavePhaseConfig { startWave = 3, endWave = 5, skeletonPercent = 0.7f, rangedImpPercent = 0.3f },
            new WavePhaseConfig { startWave = 6, endWave = 8, skeletonPercent = 0.6f, rangedImpPercent = 0.2f, meleeImpPercent = 0.2f },
            new WavePhaseConfig { startWave = 9, endWave = 11, skeletonPercent = 0.5f, rangedImpPercent = 0.2f, meleeImpPercent = 0.2f, golemPercent = 0.1f },
            new WavePhaseConfig { startWave = 12, endWave = 9999, skeletonPercent = 0.4f, rangedImpPercent = 0.2f, meleeImpPercent = 0.2f, golemPercent = 0.1f, necromongerPercent = 0.1f }
        };

        [Header("Enemy References")]
        [SerializeField] private EnemyData skeletonData;
        [SerializeField] private EnemyData rangedImpData;
        [SerializeField] private EnemyData meleeImpData;
        [SerializeField] private EnemyData golemData;
        [SerializeField] private EnemyData necromongerData;

        public int BaseEnemyCount => baseEnemyCount;
        public float EnemyCountScalingPercent => enemyCountScalingPercent;
        public float HealthScalingPercent => healthScalingPercent;
        public float DamageScalingPercent => damageScalingPercent;
        public float SpeedScalingPercent => speedScalingPercent;
        public float InitialSpawnPercent => initialSpawnPercent;
        public float TrickleSpawnPercent => trickleSpawnPercent;
        public float SpawnIntervalMin => spawnIntervalMin;
        public float SpawnIntervalMax => spawnIntervalMax;
        public float WaveBreakerDuration => waveBreakerDuration;
        public int MinSpawnPoints => minSpawnPoints;
        public int MaxSpawnPoints => maxSpawnPoints;
        public WavePhaseConfig[] WavePhases => wavePhases;
        public EnemyData SkeletonData => skeletonData;
        public EnemyData RangedImpData => rangedImpData;
        public EnemyData MeleeImpData => meleeImpData;
        public EnemyData GolemData => golemData;
        public EnemyData NecromongerData => necromongerData;

        public WavePhaseConfig GetPhaseForWave(int waveNumber)
        {
            foreach (var phase in wavePhases)
            {
                if (phase.IsValidForWave(waveNumber))
                {
                    return phase;
                }
            }
            return wavePhases[wavePhases.Length - 1];
        }

        public int CalculateEnemyCount(int waveNumber)
        {
            return Mathf.RoundToInt(baseEnemyCount * Mathf.Pow(1 + enemyCountScalingPercent, waveNumber - 1));
        }

        public float CalculateStatMultiplier(int waveNumber, float scalingPercent)
        {
            return 1f + (scalingPercent * (waveNumber - 1));
        }
    }
}

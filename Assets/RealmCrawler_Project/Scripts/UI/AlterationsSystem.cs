using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RealmCrawler.Equipment;
using RealmCrawler.CharacterStats;

namespace RealmCrawler.UI
{
    [CreateAssetMenu(fileName = "AlterationsConfig", menuName = "RealmCrawler/UI/Alterations Config")]
    public class AlterationsConfig : ScriptableObject
    {
        [Header("Cost")]
        [SerializeField] private int rerollCostPerModifier = 50;

        [Header("Modifier Pool")]
        [Tooltip("Drag ALL your SimpleStatModifier assets here. Reroll picks randomly from this pool.")]
        [SerializeField] private List<SimpleStatModifier> modifierPool = new List<SimpleStatModifier>();

        public int RerollCostPerModifier => rerollCostPerModifier;
        public IReadOnlyList<SimpleStatModifier> ModifierPool => modifierPool;
    }

    public class AlterationsSystem
    {
        private readonly AlterationsConfig config;

        public AlterationsSystem(AlterationsConfig config)
        {
            this.config = config;
            
            if (config == null)
            {
                Debug.LogError("AlterationsSystem: Config is null!");
            }
        }

        public int CalculateRerollCost(int modifierCount)
        {
            if (config == null) return 0;
            return modifierCount * config.RerollCostPerModifier;
        }

        public StatModifierBase RerollModifier(StatModifierBase currentModifier)
        {
            if (currentModifier == null)
            {
                Debug.LogWarning("AlterationsSystem: Current modifier is null");
                return null;
            }

            if (!(currentModifier is SimpleStatModifier))
            {
                Debug.LogWarning($"AlterationsSystem: Modifier type {currentModifier.GetType().Name} is not supported for rerolling");
                return null;
            }

            if (config?.ModifierPool == null || config.ModifierPool.Count == 0)
            {
                Debug.LogWarning("AlterationsSystem: Modifier pool is empty! Add SimpleStatModifier assets to the config.");
                return null;
            }

            var validModifiers = config.ModifierPool.Where(m => m != null).ToList();
            if (validModifiers.Count == 0)
            {
                Debug.LogWarning("AlterationsSystem: No valid modifiers in pool");
                return null;
            }

            SimpleStatModifier randomTemplate = validModifiers[Random.Range(0, validModifiers.Count)];

            SimpleStatModifier newModifier = ScriptableObject.CreateInstance<SimpleStatModifier>();
            newModifier.SetStat(randomTemplate.Stat);
            newModifier.SetModifierType(randomTemplate.ModType);
            newModifier.SetValue(randomTemplate.Value);

            Debug.Log($"AlterationsSystem: Rerolled to {randomTemplate.Stat?.StatName} {randomTemplate.ModType} {randomTemplate.Value}");

            return newModifier;
        }
    }
}

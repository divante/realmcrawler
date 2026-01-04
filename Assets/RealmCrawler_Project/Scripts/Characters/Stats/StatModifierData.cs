using System.Collections;
using UnityEngine;

namespace RealmCrawler.CharacterStats
{
  [CreateAssetMenu(fileName = "New Stat Modifier", menuName = "RealmCrawler/Characters/Stat Modifier")]
  public class SimpleStatModifier : StatModifierBase
  {
    [SerializeField] private StatData _stat;
    [SerializeField] private ModifierType _statType;

    public StatData Stat => _stat;
    public override ModifierType Type => _statType;

    public override void Apply(CharacterData characterData, float value, int stackCount)
    {
      float currentValue = characterData.GetStatValue(Stat);

      switch (_statType)
      {
        case ModifierType.Flat:
          currentValue += value * stackCount;
          break;
        case ModifierType.Percent:
          currentValue *= 1 + (value * stackCount);
          break;
      }

      characterData.SetStatValue(Stat, currentValue);
    }

    public override void Remove(CharacterData characterData, float value, int stackCount)
    {
      float currentValue = characterData.GetStatValue(Stat);

      switch (_statType)
      {
        case ModifierType.Flat:
          currentValue -= value * stackCount;
          break;
        case ModifierType.Percent:
          currentValue /= 1 + (value * stackCount);
          break;
      }

      characterData.SetStatValue(Stat, currentValue);
    }

  }
}
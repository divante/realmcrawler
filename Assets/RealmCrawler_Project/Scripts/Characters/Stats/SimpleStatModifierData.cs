using System;
using System.Collections;
using UnityEngine;

namespace RealmCrawler.CharacterStats
{
  public enum ModifierType { Flat, Percent }

  [CreateAssetMenu(fileName = "New Simple Stat Modifier", menuName = "RealmCrawler/Characters/Simple Stat Modifier")]
  public class SimpleStatModifier : StatModifierBase
  {
    [SerializeField] private ModifierType _statType;
    [SerializeField] protected float _value = 0f;
    public float Value => _value;
    public ModifierType ModType => _statType;
    [NonSerialized] protected new SimpleStatModifierHandler _statModifierHandlerType;

    private void OnEnable()
    {
      UpdatePhase();
    }

    private void UpdatePhase()
    {
      _modPhase = _statType == ModifierType.Flat ? ModifierPhase.PreProcess : ModifierPhase.PostProcess;
    }

    internal void SetStat(StatData stat)
    {
      _stat = stat;
    }

    internal void SetModifierType(ModifierType type)
    {
      _statType = type;
      UpdatePhase();
    }

    internal void SetValue(float value)
    {
      _value = value;
    }
  }
}
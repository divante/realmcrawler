using UnityEngine;
using System.Collections;
using System;

namespace RealmCrawler.CharacterStats
{
  // Wether the modifier should be applied before or after. Useful for scenarios where there are Flat
  // and percent modifiers, so flat are applied before all percent modifiers.
  public enum ModifierPhase { PreProcess, PostProcess }

  public abstract class StatModifierBase : ScriptableObject
  {
    [DefaultFileName]
    [SerializeField]
    public string modifierName;

    [SerializeField] protected StatData _stat;
    [SerializeField] protected Sprite _icon;
    [SerializeField] protected ParticleSystem _particleEffect;

    [NonSerialized] protected MonoBehaviour _statModifierHandlerType;
    protected ModifierPhase _modPhase;

    public StatData Stat => _stat;
    public MonoBehaviour HandlerPrefab => _statModifierHandlerType;
    public Sprite Icon => _icon;
    public ParticleSystem ParticleEffect => _particleEffect;
    public ModifierPhase Phase => _modPhase;
  }
}
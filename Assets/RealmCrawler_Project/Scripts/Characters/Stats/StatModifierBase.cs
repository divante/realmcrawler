using UnityEngine;

namespace RealmCrawler.CharacterStats
{
  public enum ModifierType { Flat, Percent }

  public abstract class StatModifierBase : ScriptableObject
  {
    [SerializeField] protected Sprite _icon;
    [SerializeField] protected ParticleSystem _particleEffect;
    [SerializeField] protected int _stackMax = 1; // -1 for infinite stacks

    public Sprite Icon => _icon;
    public ParticleSystem ParticleEffect => _particleEffect;
    public int StackMax => _stackMax;
    public abstract ModifierType Type { get; }

    public abstract void Apply(CharacterData characterData, float value, int stackCount);

    public abstract void Remove(CharacterData characterData, float value, int stackCount);
  }
}
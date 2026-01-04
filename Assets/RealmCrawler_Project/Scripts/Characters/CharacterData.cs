using System.Collections.Generic;
using UnityEngine;
using RealmCrawler.CharacterStats;
using System.Collections;

public class CharacterData : MonoBehaviour
{
  [SerializeField] private List<StatData> baseStats = new();
  private readonly Dictionary<StatData, float> currentValues = new();
  private readonly List<ActiveModifier> activeModifiers = new();
  private readonly Dictionary<ActiveModifier, Coroutine> activeModifierCoroutines = new();

  private void Awake()
  {
    InitializeStats();
  }

  private void InitializeStats()
  {
    // Initialize current values with base stats
    foreach (var stat in baseStats)
    {
      currentValues[stat] = stat.BaseValue;
    }
  }

  private void Update()
  {
    // Check for expired modifiers
    for (int i = activeModifiers.Count - 1; i >= 0; i--)
    {
      if (activeModifiers[i].IsExpired())
      {
        RemoveModifier(activeModifiers[i]);
      }
    }
  }

  public void AddModifier(StatModifierBase modifierTemplate, float value, float duration = -1)
  {
    // Check for existing stacks of this modifier
    ActiveModifier existingModifier = activeModifiers.Find(m =>
      m.ModifierTemplate == modifierTemplate);

    if (existingModifier != null)
    {
      // Handle stacking
      if (existingModifier.ModifierTemplate.StackMax == -1 ||
          existingModifier.StackCount < existingModifier.ModifierTemplate.StackMax)
      {
        existingModifier.AddStack();
        RecalculateStats();
        return;
      }
      return; // Can't stack further
    }

    // Create new active modifier
    var newModifier = new ActiveModifier(modifierTemplate, value, this, duration);
    activeModifiers.Add(newModifier);

    // Start the duration handler coroutine if duration > 0
    if (duration > 0)
    {
      Coroutine durationCoroutine = StartCoroutine(newModifier.HandleDuration());
      activeModifierCoroutines[newModifier] = durationCoroutine;
    }

    RecalculateStats();
  }

  public void RemoveModifier(ActiveModifier modifier)
  {
    // Stop the duration coroutine if it's running
    if (activeModifierCoroutines.TryGetValue(modifier, out Coroutine coroutine))
    {
      StopCoroutine(coroutine);
      activeModifierCoroutines.Remove(modifier);
    }

    activeModifiers.Remove(modifier);
    RecalculateStats();
  }

  private void RecalculateStats()
  {
    // Reset to base values
    foreach (var stat in baseStats)
    {
      currentValues[stat] = stat.BaseValue;
    }

    // Apply all active modifiers
    foreach (var modifier in activeModifiers)
    {
      modifier.ModifierTemplate.Apply(this, modifier.Value, modifier.StackCount);
    }
  }

  public float GetStatValue(StatData stat)
  {
    return currentValues.TryGetValue(stat, out float value) ? value : stat.BaseValue;
  }

  public void SetStatValue(StatData stat, float value)
  {
    currentValues[stat] = value;
  }

  // Active modifier class to track modifier state
  public class ActiveModifier
  {
    public StatModifierBase ModifierTemplate { get; }
    public float Value { get; private set; }
    public int StackCount { get; private set; }
    public float StartTime { get; private set; }
    public float Duration { get; private set; }
    private readonly CharacterData _owner;

    public ActiveModifier(StatModifierBase template, float value, CharacterData owner, float duration = -1)
    {
      ModifierTemplate = template;
      Value = value;
      StackCount = 1;
      StartTime = Time.time;
      Duration = duration;
      _owner = owner;

      // Play particle effect if available
      if (template.ParticleEffect != null)
      {
        var effect = Instantiate(template.ParticleEffect, _owner.transform.position, Quaternion.identity);
        effect.Play();
        Destroy(effect.gameObject, effect.main.duration);
      }
    }

    public void AddStack()
    {
      StackCount++;
    }

    public bool IsExpired()
    {
      return Duration >= 0 && Time.time - StartTime >= Duration;
    }

    public IEnumerator HandleDuration()
    {
      // Wait for the duration
      yield return new WaitForSeconds(Duration);

      // Remove the modifier
      _owner.RemoveModifier(this);
    }
  }
}
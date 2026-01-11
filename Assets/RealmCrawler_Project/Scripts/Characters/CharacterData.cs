using System;
using System.Collections.Generic;
using UnityEngine;
using RealmCrawler.CharacterStats;
using System.Collections.Concurrent;

public class CharacterData : MonoBehaviour
{
  [SerializeField] private List<StatData> baseStats = new();
  private readonly ConcurrentDictionary<Guid, float> currentValues = new();
  private readonly List<StatModifierHandlerBase> preModifiers = new();
  private readonly List<StatModifierHandlerBase> postModifiers = new();


  private bool _isDirty = true;
  private void Awake()
  {
    InitializeStats();
  }

  private void InitializeStats()
  {
    if (baseStats == null) return;
    // Debug.Log($"CharacterData.InitializeStats: Initializing {baseStats.Count} stats");
    foreach (var stat in baseStats)
    {
      currentValues[stat.UUID] = stat.BaseValue;
      // Debug.Log($"CharacterData.InitializeStats: {stat.StatName} - UUID: {stat.UUID}, BaseValue: {stat.BaseValue}");
    }
  }

  private void Update()
  {
    if (!_isDirty) return;
    RecalculateStats();
  }

  public void AddModifier(StatModifierBase modifier)
  {
    if (modifier == null)
    {
      Debug.LogWarning("CharacterData.AddModifier: modifier is null");
      return;
    }

    Debug.Log($"CharacterData.AddModifier: Adding modifier {modifier.name}, Type: {modifier.GetType().Name}");

    GameObject handlerObj = new GameObject($"{modifier.name}_Handler");
    handlerObj.transform.SetParent(transform);

    StatModifierHandlerBase newModifierHandler = null;

    if (modifier is SimpleStatModifier)
    {
      newModifierHandler = handlerObj.AddComponent<SimpleStatModifierHandler>();
      Debug.Log($"CharacterData.AddModifier: Created SimpleStatModifierHandler for {modifier.name}");
    }
    else
    {
      Debug.LogError($"Unknown modifier type for modifier: {modifier.name}");
      Destroy(handlerObj);
      return;
    }

    if (newModifierHandler == null)
    {
      Debug.LogError($"Failed to create handler for modifier: {modifier.name}");
      Destroy(handlerObj);
      return;
    }
    newModifierHandler.Initialize(modifier, this);
    if (modifier.Phase.Equals(ModifierPhase.PreProcess))
      preModifiers.Add(newModifierHandler);
    else
      postModifiers.Add(newModifierHandler);

    newModifierHandler.Apply();
    _isDirty = true;
    Debug.Log($"CharacterData.AddModifier: Successfully added modifier {modifier.name}, Phase: {modifier.Phase}");
  }

  public void RemoveModifier(StatModifierHandlerBase modifier)
  {
    if (!preModifiers.Remove(modifier) && !postModifiers.Remove(modifier))
    {
      Debug.LogWarning("Modifier not found: " + modifier.ToString());
      return;
    }

    modifier.Remove();
    Destroy(modifier.gameObject);
    _isDirty = true;
  }

  private void RecalculateStats()
  {
    // Debug.Log("CharacterData.RecalculateStats: Starting recalculation");
    // Reset to base values
    foreach (var stat in baseStats)
    {
      currentValues[stat.UUID] = stat.BaseValue;
      // Debug.Log($"CharacterData.RecalculateStats: Reset {stat.StatName} to {stat.BaseValue}");
    }

    // Apply pre-process modifiers
    foreach (var modifier in preModifiers)
    {
      modifier.Apply();
    }

    // Apply post-process modifiers
    foreach (var modifier in postModifiers)
    {
      modifier.Apply();
    }

    _isDirty = false;
    Debug.Log("CharacterData.RecalculateStats: Finished recalculation");
  }

  public float GetStatValue(StatData stat)
  {
    float value = currentValues.TryGetValue(stat.UUID, out float v) ? v : stat.BaseValue;
    Debug.Log($"CharacterData.GetStatValue: {stat.StatName} - UUID: {stat.UUID}, currentValues has key: {currentValues.ContainsKey(stat.UUID)}, returned value: {value}");
    return value;
  }

  public void SetStatValue(StatData stat, float value)
  {
    if (value.Equals(currentValues[stat.UUID])) return;

    currentValues[stat.UUID] = value;
    _isDirty = true;
  }
}
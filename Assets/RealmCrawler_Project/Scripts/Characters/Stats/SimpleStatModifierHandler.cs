using System.Collections;
using UnityEngine;

namespace RealmCrawler.CharacterStats
{
  public class SimpleStatModifierHandler : StatModifierHandlerBase
  {
    protected new SimpleStatModifier _modifier;

    public override void Initialize(StatModifierBase modifier, CharacterData characterData)
    {
      base.Initialize(modifier, characterData);
      _modifier = modifier as SimpleStatModifier;
    }

    public override void Apply()
    {
      if (_modifier == null)
      {
        Debug.LogError("SimpleStatModifierHandler.Apply: _modifier is null");
        return;
      }

      if (_modifier.Stat == null)
      {
        Debug.LogError($"SimpleStatModifierHandler.Apply: Stat is null on modifier {_modifier.name}");
        return;
      }

      float currentVal = _characterData.GetStatValue(_modifier.Stat);
      Debug.Log($"SimpleStatModifierHandler.Apply: Applying {_modifier.name} ({_modifier.ModType}) to {_modifier.Stat.StatName}. Current: {currentVal}, Modifier value: {_modifier.Value}");

      if (_modifier.ModType == ModifierType.Flat)
      {
        float newValue = currentVal + _modifier.Value;
        _characterData.SetStatValue(_modifier.Stat, newValue);
        Debug.Log($"SimpleStatModifierHandler.Apply: Flat modifier applied. New value: {newValue}");
      }
      else if (_modifier.ModType == ModifierType.Percent)
      {
        float newValue = currentVal * (1 + _modifier.Value);
        _characterData.SetStatValue(_modifier.Stat, newValue);
        Debug.Log($"SimpleStatModifierHandler.Apply: Percent modifier applied. New value: {newValue}");
      }

      _isActive = true;
    }

    public override void Remove()
    {
      _isActive = false;

    }
  }
}
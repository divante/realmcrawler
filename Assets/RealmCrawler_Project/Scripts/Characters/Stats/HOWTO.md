# RealmCrawler Character Stat System HOWTO

## Overview
This stat system provides a flexible framework for managing character attributes with support for modifiers, stacking, and different application phases.

## Core Components

### 1. StatData (ScriptableObject)
- **Purpose**: Defines base character attributes
- **Creation**:
  - Right-click in Project window → Create → RealmCrawler → Characters → Stat
  - Name will be automatically set from filename
  - Unique ID is automatically generated

### 2. StatModifierBase (Abstract)
- **Purpose**: Base class for all stat modifiers
- **Implementation**:
  - Create new ScriptableObject that inherits from StatModifierBase
  - Implement specific modifier logic in derived classes

### 3. StatModifierHandlerBase (Abstract MonoBehaviour)
- **Purpose**: Runtime handler for modifiers
- **Implementation**:
  - Create MonoBehaviour that inherits from StatModifierHandlerBase
  - Implement Apply() and Remove() methods

### 4. CharacterData (MonoBehaviour)
- **Purpose**: Manages all stats and modifiers for a character
- **Usage**:
  - Attach to GameObject representing a character
  - Configure base stats in inspector

## Creating a New Stat

1. Right-click in Project window
2. Select Create → RealmCrawler → Characters → Stat
3. Name your stat (e.g., "Strength", "Health")
4. Set base value in inspector
5. Assign icon if desired

## Creating a New Modifier

### Example: Simple Stat Modifier

1. Create new ScriptableObject inheriting from StatModifierBase
2. Add fields for:
   - Target stat (StatData reference)
   - Modifier type (Flat or Percent)
   - Modifier value
3. Create corresponding handler MonoBehaviour
4. Implement Apply() and Remove() logic

### Implementation Example:

```csharp
// SimpleStatModifier.cs
[CreateAssetMenu(fileName = "New Simple Stat Modifier", menuName = "RealmCrawler/Characters/Simple Stat Modifier")]
public class SimpleStatModifier : StatModifierBase
{
    [SerializeField] private ModifierType _statType;
    [SerializeField] protected float _value = 0f;
    public float Value => _value;
    private new SimpleStatModifierHandler _statModifierHandlerType;
}

// SimpleStatModifierHandler.cs
public class SimpleStatModifierHandler : StatModifierHandlerBase
{
    protected new SimpleStatModifier _modifier;

    public override void Apply()
    {
        float currentVal = _characterData.GetStatValue(_modifier.Stat);
        if (_modifier.Stat.GetType().Equals(ModifierType.Flat))
        {
            _characterData.SetStatValue(_modifier.Stat, currentVal + _modifier.Value);
        }
        else if (_modifier.Stat.GetType().Equals(ModifierType.Percent))
        {
            _characterData.SetStatValue(_modifier.Stat, currentVal * (1 + _modifier.Value));
        }
        _isActive = true;
    }

    public override void Remove()
    {
        _isActive = false;
    }
}
```

## Using Modifiers

### Adding Modifiers
```csharp
// Get reference to CharacterData component
CharacterData characterData = GetComponent<CharacterData>();

// Create or get your modifier
SimpleStatModifier strengthBoost = ScriptableObject.CreateInstance<SimpleStatModifier>();
// Configure modifier properties...

// Add to character
characterData.AddModifier(strengthBoost);
```

### Removing Modifiers
```csharp
// Remove all modifiers of a specific type
foreach (var modifier in characterData.GetActiveModifiers<SimpleStatModifierHandler>())
{
    characterData.RemoveModifier(modifier);
}
```

## Best Practices

1. **Modifier Organization**:
   - Group related modifiers in folders
   - Use consistent naming conventions

2. **Performance**:
   - Minimize modifier count on single characters
   - Consider pooling modifier handlers

3. **Debugging**:
   - Use the IsActive property to track modifier state
   - Implement logging for modifier application/removal

4. **Extensibility**:
   - Create new modifier types for complex interactions
   - Implement modifier stacking rules as needed

## Advanced Features

### Modifier Phases
The system supports different application phases:
- PreProcess: Applied before other modifiers
- PostProcess: Applied after other modifiers

### Stacking
Implement stacking logic in your modifier handlers:
```csharp
public override void Apply()
{
    // Check for existing modifiers of same type
    var existingModifiers = _characterData.GetActiveModifiers<SimpleStatModifierHandler>()
        .Where(m => m.Modifier == _modifier);

    if (existingModifiers.Any())
    {
        // Stack with existing modifiers
        var firstModifier = existingModifiers.First();
        _characterData.SetStatValue(_modifier.Stat,
            _characterData.GetStatValue(_modifier.Stat) + _modifier.Value);
    }
    else
    {
        // Apply new modifier
        base.Apply();
    }
}
```

## Troubleshooting

1. **Modifiers not applying**:
   - Verify handler type matches modifier type
   - Check that CharacterData component exists
   - Ensure modifier is properly configured

2. **Stats not updating**:
   - Check for dirty flag issues
   - Verify RecalculateStats() is being called
   - Ensure no null references in modifier configuration

3. **Performance issues**:
   - Reduce modifier count
   - Consider optimizing modifier application logic
   - Profile to identify bottlenecks
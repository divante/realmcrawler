using UnityEngine;
using System.Collections;

namespace RealmCrawler.CharacterStats
{
  public abstract class StatModifierHandlerBase : MonoBehaviour
  {
    protected StatModifierBase _modifier;
    protected CharacterData _characterData;
    protected bool _isActive = false;

    public bool IsActive => _isActive;

    public virtual void Initialize(StatModifierBase modifier, CharacterData characterData)
    {
      _modifier = modifier;
      _characterData = characterData;
    }

    public abstract void Apply();

    public abstract void Remove();

  }
}
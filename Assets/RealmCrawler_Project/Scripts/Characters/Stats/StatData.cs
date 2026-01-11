using UnityEngine;
using System;

namespace RealmCrawler.CharacterStats
{
  [CreateAssetMenu(fileName = "New Stat", menuName = "RealmCrawler/Characters/Stat")]
  public class StatData : ScriptableObject
  {
    [SerializeField] private Guid uuid = Guid.NewGuid();
    [DefaultFileName][SerializeField] private string statName;
    [SerializeField] private float baseValue;
    [SerializeField] protected Sprite _icon;

    public string StatName => statName;
    public float BaseValue => baseValue;
    public Guid UUID => uuid;
  }
}

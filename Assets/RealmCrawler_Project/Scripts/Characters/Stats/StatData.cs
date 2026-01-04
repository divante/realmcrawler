using UnityEngine;
using UnityEditor;
using System;
using Unity.VisualScripting;


namespace RealmCrawler.CharacterStats
{

  [CreateAssetMenu(fileName = "New Stat", menuName = "RealmCrawler/Characters/Stat")]
  public class StatData : ScriptableObject
  {
    [SerializeField] private Guid uuid;     // Unique identifier for the stat
    [SerializeField] private string statName;     // “Health”, “FireDamage”, …
    [SerializeField] private float baseValue;    // Base value for a fresh run.\

    public Guid UUID => uuid;
    public string StatName => statName;
    public float BaseValue => baseValue;

    // Internal method to set the unique ID
    internal void SetUUID(Guid id)
    {
      uuid = id;
    }

    // Internal method to allow the editor to set the statName
    internal void SetStatName(string name)
    {
      statName = name;
    }
  }

  [CustomEditor(typeof(StatData))]
  [CanEditMultipleObjects]
  public class StatTemplateEditor : Editor
  {
    private void OnEnable()
    {
      // Check if this is a new asset (no statName set yet)
      StatData statTemplate = (StatData)target;
      if (string.IsNullOrEmpty(statTemplate.StatName))
      {
        // Get the asset path
        string path = AssetDatabase.GetAssetPath(statTemplate);
        if (!string.IsNullOrEmpty(path))
        {
          // Remove the file extension and set as statName
          string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
          Undo.RecordObject(statTemplate, "Set Stat Name");
          statTemplate.SetStatName(fileName);
          EditorUtility.SetDirty(statTemplate);
        }

      }
      // Generate and set a unique ID if not already set
      if (statTemplate.UUID == Guid.Empty)
      {
        statTemplate.SetUUID(Guid.NewGuid());
        EditorUtility.SetDirty(statTemplate);
      }

    }
  }
}

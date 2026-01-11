using UnityEngine;
using UnityEditor;
using System;
using System.Linq;


[CustomPropertyDrawer(typeof(DefaultFileNameAttribute))]
public class DefaultFileName : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    if (string.IsNullOrEmpty(property.stringValue))
    {
      string path = AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
      string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
      Debug.Log("default filename for " + fileName);
      property.stringValue = fileName;

    }

    EditorGUI.BeginProperty(position, label, property);
    Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
    EditorGUI.LabelField(labelRect, label);
    EditorGUI.PropertyField(position, property, label);
    EditorGUI.EndProperty();
  }
}

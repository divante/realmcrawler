using UnityEngine;
using UnityEditor;
using System;
using System.Linq;


[CustomPropertyDrawer(typeof(SelectImplementationAttribute))]
public class SelectImplementationDrawer : PropertyDrawer
{
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
  {
    return EditorGUI.GetPropertyHeight(property, label, true);
  }

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);

    // Draw the label
    Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
    EditorGUI.LabelField(labelRect, label);

    // Draw the dropdown button
    Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

    var currentManagedType = GetManagedType(property);
    string typeName = currentManagedType != null ? currentManagedType.Name : "(Null) Select Type...";

    if (GUI.Button(buttonRect, typeName, EditorStyles.popup))
    {
      ShowTypeSelectorMenu(property);
    }

    // Draw the content below if it exists
    if (property.managedReferenceValue != null)
    {
      EditorGUI.PropertyField(position, property, GUIContent.none, true);
    }

    EditorGUI.EndProperty();
  }

  private void ShowTypeSelectorMenu(SerializedProperty property)
  {
    GenericMenu menu = new GenericMenu();
    Type baseType = GetFieldType(property);

    // Find all non-abstract classes that inherit from baseType
    var types = TypeCache.GetTypesDerivedFrom(baseType)
        .Where(p => !p.IsAbstract && !p.IsInterface)
        .OrderBy(p => p.Name);

    // "None" option
    menu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () =>
    {
      property.managedReferenceValue = null;
      property.serializedObject.ApplyModifiedProperties();
    });

    // Derived Types options
    foreach (var type in types)
    {
      menu.AddItem(new GUIContent(type.Name), type == GetManagedType(property), () =>
      {
        property.managedReferenceValue = Activator.CreateInstance(type);
        property.serializedObject.ApplyModifiedProperties();
      });
    }

    menu.ShowAsContext();
  }

  private Type GetManagedType(SerializedProperty property)
  {
    if (property.managedReferenceValue == null) return null;
    return property.managedReferenceValue.GetType();
  }

  private Type GetFieldType(SerializedProperty property)
  {
    if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
    {
      return fieldInfo.FieldType.GetGenericArguments()[0];
    }
    if (fieldInfo.FieldType.IsArray)
    {
      return fieldInfo.FieldType.GetElementType();
    }
    return fieldInfo.FieldType;
  }
}
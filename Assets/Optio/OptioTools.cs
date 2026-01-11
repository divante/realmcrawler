using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public static class OptioTools
{
  // TOOL 1: FIND_CLASS
  // Returns the path of a script defining a specific class/interface
  public static string FindClassPath(string className)
  {
    // 1. Fast Filename Search
    string[] guids = AssetDatabase.FindAssets($"{className} t:Script");
    foreach (string guid in guids)
    {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      if (path.StartsWith("Assets/") && Path.GetFileNameWithoutExtension(path) == className)
        return path;
    }

    // 2. Deep Metadata Search (For Interfaces/Mismatched names)
    MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
    foreach (MonoScript script in scripts)
    {
      var type = script.GetClass();
      if (type != null && type.Name == className)
        return AssetDatabase.GetAssetPath(script);
    }

    return "NOT_FOUND";
  }

  // TOOL 2: READ_FILE
  public static string ReadFile(string path)
  {
    if (!File.Exists(path)) return $"ERROR: File not found at {path}";
    return File.ReadAllText(path);
  }

  // TOOL 3: WRITE_FILE
  public static void WriteFile(string path, string content)
  {
    // Security: Force Assets path
    if (!path.StartsWith("Assets"))
      path = Path.Combine("Assets/Generated", Path.GetFileName(path));

    string dir = Path.GetDirectoryName(path);
    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

    File.WriteAllText(path, content.Trim());
    AssetDatabase.Refresh();
  }
}
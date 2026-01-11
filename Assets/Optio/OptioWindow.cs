using UnityEngine;
using UnityEditor;

public class OptioWindow : EditorWindow
{
  private OptioAgent _agent;
  private string _userPrompt = "Refactor PlayerController to use StateMachine.";
  private string _status = "Ready.";
  private bool _isWorking = false;

  [MenuItem("Optio/Agent Window")]
  public static void ShowWindow() => GetWindow<OptioWindow>("Optio Agent");

  private void OnEnable()
  {
    // Initialize the Agent
    _agent = new OptioAgent("http://nightshift:11434", "qwen-standard");
    _agent.OnStatusChange += (msg) => { _status = msg; Repaint(); };
  }

  private void OnGUI()
  {
    GUILayout.Label("Optio Agent v3.0", EditorStyles.boldLabel);

    _userPrompt = EditorGUILayout.TextArea(_userPrompt, GUILayout.Height(100));

    if (!_isWorking && GUILayout.Button("Run Agent", GUILayout.Height(40)))
    {
      RunAgent();
    }

    EditorGUILayout.HelpBox(_status, MessageType.Info);
  }

  private async void RunAgent()
  {
    _isWorking = true;

    // Grab selected file content as initial context hint
    string context = "";
    if (Selection.activeObject != null)
    {
      string path = AssetDatabase.GetAssetPath(Selection.activeObject);
      if (path.EndsWith(".cs"))
        context = $"I have selected this file: {path}";
    }

    await _agent.RunTask(_userPrompt, context);
    _isWorking = false;
  }
}
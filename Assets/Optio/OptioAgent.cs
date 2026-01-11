using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

public class OptioAgent
{
  private OptioNetwork _net;
  private StringBuilder _conversationLog = new StringBuilder();
  private int _maxSteps = 5; // Prevent infinite loops

  public System.Action<string> OnStatusChange; // Callback for UI updates

  public OptioAgent(string url, string model)
  {
    _net = new OptioNetwork(url, model);
  }

  public async Task RunTask(string userGoal, string initialContext = "")
  {
    _conversationLog.Clear();
    _conversationLog.AppendLine($"GOAL: {userGoal}");
    if (!string.IsNullOrEmpty(initialContext))
      _conversationLog.AppendLine($"CONTEXT:\n{initialContext}");

    int steps = 0;
    bool taskComplete = false;

    // start a timer
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    stopwatch.Start();


    while (!taskComplete && steps < _maxSteps)
    {
      steps++;
      OnStatusChange?.Invoke($"Thinking (Step {steps}/{_maxSteps})...");

      // 1. Ask the LLM
      string response = await _net.SendRequest(GetSystemPrompt(), _conversationLog.ToString());
      Debug.Log("got a response - " + response);
      if (string.IsNullOrEmpty(response)) break;

      _conversationLog.AppendLine($"\nAGENT:\n{response}");

      // 2. Check for Tools
      if (response.Contains("<cmd>"))
      {
        OnStatusChange?.Invoke($"Executing Tools...");
        string observation = ProcessCommands(response);
        _conversationLog.AppendLine($"\nSYSTEM_OBSERVATION:\n{observation}");
      }
      else
      {
        // No commands means the Agent is done or asking a question
        taskComplete = true;

        // Final Parsing: Check for File writes using the XML format
        ProcessFileWrites(response);
      }
    }

    stopwatch.Start();

    OnStatusChange?.Invoke("Task Complete. Took " + stopwatch.ElapsedMilliseconds + " ms.");

  }

  private string ProcessCommands(string llmResponse)
  {
    var matches = Regex.Matches(llmResponse, @"<cmd>(.*?)</cmd>");
    StringBuilder observations = new StringBuilder();

    foreach (Match match in matches)
    {
      string rawCmd = match.Groups[1].Value.Trim();
      string[] parts = rawCmd.Split(' ');
      string action = parts[0].ToUpper();
      string arg = parts.Length > 1 ? rawCmd.Substring(action.Length).Trim() : "";

      switch (action)
      {
        case "FIND":
          string path = OptioTools.FindClassPath(arg);
          observations.AppendLine($"[FIND {arg}] => {path}");
          break;
        case "READ":
          string content = OptioTools.ReadFile(arg);
          observations.AppendLine($"[READ {arg}] =>\n{content}");
          break;
        default:
          observations.AppendLine($"[ERROR] Unknown command: {action}");
          break;
      }
    }
    return observations.ToString();
  }

  private void ProcessFileWrites(string llmResponse)
  {
    // Parse <file path="..."> content </file>
    Debug.Log(" processing response: " + llmResponse);
    var matches = Regex.Matches(llmResponse, @"<file path=""(.*?)"">\s*(.*?)\s*</file>", RegexOptions.Singleline);
    foreach (Match match in matches)
    {
      string path = match.Groups[1].Value;
      string code = match.Groups[2].Value;
      OptioTools.WriteFile(path, code);
    }
  }

  private string GetSystemPrompt()
  {
    return @"You are an Autonomous Unity Coding Agent.
        
        TOOLS:
        1. <cmd>FIND ClassName</cmd> : Finds the file path of a class or interface.
        2. <cmd>READ Assets/Path/To/File.cs</cmd> : Reads the file content.
        
        PROTOCOL:
        - If you need to edit a class, FIND it, then READ it first.
        - Do not guess code. Verify via READ.
        - You can perform multiple commands in one turn.
        
        FINAL OUTPUT:
        When you are ready to write code, output it wrapped in XML tags:
        <file path=""Assets/Scripts/MyFile.cs""> ... code ... </file>";
  }
}
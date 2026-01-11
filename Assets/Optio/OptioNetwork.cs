using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;

public class OptioNetwork
{
  private string _baseUrl;
  private string _model;
  private UnityWebRequest _currentRequest;

  public OptioNetwork(string url, string model)
  {
    // Remove trailing slash to prevent double-slashes in URL
    _baseUrl = url.TrimEnd('/');
    _model = model;
  }

  public void Abort()
  {
    if (_currentRequest != null)
    {
      _currentRequest.Abort();
      _currentRequest.Dispose();
      _currentRequest = null;
    }
  }

  [System.Serializable]
  public class AgentSettings
  {
    public float temperature = 0.1f;
    public int max_tokens = 32768; // This acts as a limit, not allocation
    public float top_p = 0.9f;
    public float min_p = 0.05f;
  }

  public async Task<string> SendRequest(string systemPrompt, string userConversation, AgentSettings settings = null)
  {
    if (settings == null) settings = new();
    // ---------------------------------------------------------
    // CHANGE 1: Use the OpenAI Standard Endpoint
    // Llama.cpp uses this. Ollama SUPPORTS this too (v0.1.24+)
    // ---------------------------------------------------------
    string endpoint = $"{_baseUrl}/v1/chat/completions";

    // ---------------------------------------------------------
    // CHANGE 2: OpenAI JSON Format
    // - No "options" object (parameters are top-level)
    // - "max_tokens" instead of "num_ctx" (sometimes ignored by server, but good practice)
    // ---------------------------------------------------------
    string jsonPayload = $@"
    {{
        ""model"": ""{_model}"",
        ""messages"": [ ... ],
        ""temperature"": {settings.temperature},
        ""max_tokens"": {settings.max_tokens},
        ""top_p"": {settings.top_p},
        ""min_p"": {settings.min_p},
        ""stream"": false
    }}";

    try
    {
      _currentRequest = new UnityWebRequest(endpoint, "POST");
      byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
      _currentRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
      _currentRequest.downloadHandler = new DownloadHandlerBuffer();
      _currentRequest.SetRequestHeader("Content-Type", "application/json");

      // Llama.cpp often requires an Auth header, even if fake
      _currentRequest.SetRequestHeader("Authorization", "Bearer optio-local");

      var operation = _currentRequest.SendWebRequest();

      while (!operation.isDone) await Task.Yield();

      if (_currentRequest.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError($"[OptioNet] Error: {_currentRequest.error}\nResponse: {_currentRequest.downloadHandler.text}");
        return null;
      }

      // ---------------------------------------------------------
      // CHANGE 3: Parse OpenAI Response Format
      // Structure: { "choices": [ { "message": { "content": "..." } } ] }
      // ---------------------------------------------------------
      string responseJson = _currentRequest.downloadHandler.text;
      return ExtractContentFromOpenAI(responseJson);
    }
    catch (System.Exception ex)
    {
      Debug.LogError($"[OptioNet] Exception: {ex.Message}");
      return null;
    }
    finally
    {
      _currentRequest?.Dispose();
      _currentRequest = null;
    }
  }

  private string ExtractContentFromOpenAI(string json)
  {
    // Using Unity's JsonUtility with wrapper classes
    var responseObj = JsonUtility.FromJson<OpenAIResponse>(json);
    if (responseObj != null && responseObj.choices != null && responseObj.choices.Length > 0)
    {
      return responseObj.choices[0].message.content;
    }
    return null;
  }

  private string EscapeJson(string str) => str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "").Replace("\t", "\\t");

  // Wrapper Classes for JSON Parsing
  [System.Serializable] class OpenAIResponse { public OpenAIChoice[] choices; }
  [System.Serializable] class OpenAIChoice { public OpenAIMessage message; }
  [System.Serializable] class OpenAIMessage { public string content; }
}
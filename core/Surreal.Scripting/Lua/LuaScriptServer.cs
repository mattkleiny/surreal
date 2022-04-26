namespace Surreal.Scripting.Lua;

/// <summary>A <see cref="IScriptServer"/> implementation for Lua, based on MoonSharp.</summary>
public sealed class LuaScriptServer : IScriptServer
{
  private readonly ConcurrentDictionary<ScriptHandle, ScriptEntry> scriptsByHandle = new();
  private int nextScriptId = 0;

  public ScriptHandle CreateScript()
  {
    var id = new ScriptHandle(Interlocked.Increment(ref nextScriptId));

    scriptsByHandle[id] = new();

    return id;
  }

  public void CompileScriptCode(ScriptHandle handle, string code)
  {
    if (!scriptsByHandle.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    entry.Code = code;
    entry.Script.DoString(code);
  }

  public dynamic ExecuteScript(ScriptHandle handle)
  {
    if (!scriptsByHandle.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    return entry.Script.DoString(entry.Code);
  }

  public dynamic ExecuteScriptFunction(ScriptHandle handle, string functionName, dynamic[] parameters)
  {
    if (!scriptsByHandle.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    var function = entry.Script.Globals[functionName];
    if (function == null)
    {
      throw new InvalidOperationException($"Unable to access function {functionName} in script {handle}");
    }

    return entry.Script.Call(function, parameters);
  }

  public void DeleteScript(ScriptHandle handle)
  {
    scriptsByHandle.TryRemove(handle, out _);
  }

  private sealed record ScriptEntry
  {
    public MoonSharp.Interpreter.Script Script { get; }      = new();
    public string                       Code   { get; set; } = string.Empty;
  }
}

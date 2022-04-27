using NLua;
using Surreal.Diagnostics.Logging;

namespace Surreal.Scripting;

/// <summary>A <see cref="IScriptServer"/> implementation for Lua, based on MoonSharp.</summary>
public sealed class LuaScriptServer : IScriptServer, IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<LuaScriptServer>();

  private readonly Dictionary<ScriptHandle, ScriptEntry> scripts = new();
  private int nextScriptId = 0;

  public ScriptHandle CreateScript()
  {
    var handle = new ScriptHandle(Interlocked.Increment(ref nextScriptId));
    var entry = new ScriptEntry();

    scripts[handle] = entry;

    entry.Runtime.LoadCLRPackage();

    return handle;
  }

  public void CompileScriptCode(ScriptHandle handle, string code)
  {
    if (!scripts.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    try
    {
      entry.Code = code;
      entry.Runtime.DoString(code);
    }
    catch (Exception exception)
    {
      Log.Error(exception, "Failed to compile script code");
    }
  }

  public object? ExecuteScript(ScriptHandle handle)
  {
    if (!scripts.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    try
    {
      return entry.Runtime.DoString(entry.Code);
    }
    catch (Exception exception)
    {
      Log.Error(exception, $"Failed to execute script {handle}");

      return null;
    }
  }

  public object? ExecuteScriptFunction(ScriptHandle handle, string functionName)
  {
    return ExecuteScriptFunction(handle, functionName, Array.Empty<object>());
  }

  public object? ExecuteScriptFunction(ScriptHandle handle, string functionName, object[] parameters)
  {
    if (!scripts.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    try
    {
      var function = entry.Runtime.GetFunction(functionName);
      var result = function.Call(parameters);

      if (result is not { Length: > 0 })
      {
        return null;
      }

      return result[0];
    }
    catch (Exception exception)
    {
      Log.Error(exception, $"Failed to execute script function {functionName} in script {handle}");

      return null;
    }
  }

  public void DeleteScript(ScriptHandle handle)
  {
    if (scripts.TryGetValue(handle, out var entry))
    {
      entry.Dispose();
      scripts.Remove(handle);
    }
  }

  public void Dispose()
  {
    foreach (var entry in scripts.Values)
    {
      entry.Dispose();
    }

    scripts.Clear();
  }

  private sealed record ScriptEntry : IDisposable
  {
    public Lua    Runtime { get; }      = new();
    public string Code    { get; set; } = string.Empty;

    public void Dispose()
    {
      Runtime.Dispose();
    }
  }
}

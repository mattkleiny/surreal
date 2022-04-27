using NLua;

namespace Surreal.Scripting;

/// <summary>A <see cref="IScriptServer"/> implementation for Lua, based on MoonSharp.</summary>
public sealed class LuaScriptServer : IScriptServer, IDisposable
{
  private readonly Dictionary<ScriptHandle, ScriptEntry> scripts = new();
  private int nextScriptId = 0;

  public ScriptHandle CreateScript()
  {
    var handle = new ScriptHandle(Interlocked.Increment(ref nextScriptId));

    scripts[handle] = new ScriptEntry();

    return handle;
  }

  public void CompileScriptCode(ScriptHandle handle, string code)
  {
    if (!scripts.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    entry.Code = code;
    entry.Runtime.DoString(code);
  }

  public object? ExecuteScript(ScriptHandle handle)
  {
    if (!scripts.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    return entry.Runtime.DoString(entry.Code);
  }

  public object? ExecuteScriptFunction(ScriptHandle handle, string functionName)
  {
    if (!scripts.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    var function = entry.Runtime.GetFunction(functionName);
    var result = function.Call(Array.Empty<object>());

    return result?[0];
  }

  public object? ExecuteScriptFunction(ScriptHandle handle, string functionName, object[] parameters)
  {
    if (!scripts.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    var function = entry.Runtime.GetFunction(functionName);
    var result = function.Call(parameters);

    if (result is not { Length: > 0 })
    {
      return null;
    }

    return result[0];
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

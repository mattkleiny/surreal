using NLua;
using Surreal.Diagnostics.Logging;
using Surreal.IO;

namespace Surreal.Scripting;

/// <summary>A <see cref="IScriptServer" /> implementation for Lua, based on MoonSharp.</summary>
public sealed class LuaScriptServer : IScriptServer, IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<LuaScriptServer>();

  private readonly Dictionary<ScriptHandle, ScriptEntry> _scripts = new();
  private int _nextScriptId = 0;

  public void Dispose()
  {
    foreach (var entry in _scripts.Values) entry.Dispose();

    _scripts.Clear();
  }

  public ScriptHandle CreateScript()
  {
    var handle = new ScriptHandle(Interlocked.Increment(ref _nextScriptId));
    var entry = new ScriptEntry();

    _scripts[handle] = entry;

    entry.Runtime.LoadCLRPackage();

    // replace the default 'print' function
    RegisterFunction(handle, "print", (string message) =>
    {
      if (entry.Path != null)
      {
        Log.Trace($"[{entry.Path.Value}]: {message}");
      }
      else
      {
        Log.Trace($"[Script {handle.Id}]: {message}");
      }
    });

    // replace the default 'require' function
    RegisterFunction(handle, "require", (string uri) =>
    {
      var path = VirtualPath.Parse(uri);
      var code = path.ReadAllText();

      entry.Runtime.DoString(code);
    });

    return handle;
  }

  public void CompileScriptCode(ScriptHandle handle, string code, VirtualPath sourcePath)
  {
    if (!_scripts.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    try
    {
      entry.Path = sourcePath;
      entry.Code = code;
      entry.Runtime.DoString(code);
    }
    catch (Exception exception)
    {
      Log.Error(exception, "Failed to compile script code");
    }
  }

  public void RegisterFunction(ScriptHandle handle, string name, Delegate callback)
  {
    if (!_scripts.TryGetValue(handle, out var entry))
    {
      throw new InvalidOperationException($"Unable to access script for handle {handle}");
    }

    entry.Runtime.RegisterFunction(name, callback.Target, callback.Method);
  }

  public object? ExecuteScript(ScriptHandle handle)
  {
    if (!_scripts.TryGetValue(handle, out var entry))
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
    if (!_scripts.TryGetValue(handle, out var entry))
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
    if (_scripts.TryGetValue(handle, out var entry))
    {
      entry.Dispose();
      _scripts.Remove(handle);
    }
  }

  private sealed record ScriptEntry : IDisposable
  {
    public VirtualPath? Path { get; set; }
    public Lua Runtime { get; } = new();
    public string Code { get; set; } = string.Empty;

    public void Dispose()
    {
      Runtime.Dispose();
    }
  }
}




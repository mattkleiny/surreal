namespace Surreal.Scripting;

/// <summary>An opaque handle to a resource in the underling <see cref="IScriptServer"/> implementation.</summary>
public readonly record struct ScriptHandle(nint Id)
{
  public static implicit operator nint(ScriptHandle handle) => handle.Id;
  public static implicit operator int(ScriptHandle handle) => (int)handle.Id;
  public static implicit operator uint(ScriptHandle handle) => (uint)handle.Id;
}

/// <summary>An abstraction over the different types of scripting servers available.</summary>
public interface IScriptServer
{
  ScriptHandle CreateScript();
  void CompileScriptCode(ScriptHandle handle, string code);
  dynamic? ExecuteScript(ScriptHandle handle);
  dynamic? ExecuteScriptFunction(ScriptHandle handle, string functionName, dynamic[] parameters);
  void DeleteScript(ScriptHandle handle);
}

namespace Surreal.Scripting;

/// <summary>A no-op <see cref="IScriptServer"/> for headless environments and testing.</summary>
public sealed class HeadlessScriptServer : IScriptServer
{
  private int nextScriptId = 0;

  public ScriptHandle CreateScript()
  {
    return new ScriptHandle(Interlocked.Increment(ref nextScriptId));
  }

  public void CompileScriptCode(ScriptHandle handle, string code)
  {
    // no-op
  }

  public dynamic? ExecuteScript(ScriptHandle handle)
  {
    return null;
  }

  public dynamic? ExecuteScriptFunction(ScriptHandle handle, string functionName, dynamic[] parameters)
  {
    return null;
  }

  public void DeleteScript(ScriptHandle handle)
  {
    // no-op
  }
}

namespace Surreal.Scripting;

/// <summary>
/// A script asset, created from a <see cref="IScriptLanguage"/>.
/// </summary>
public abstract class Script : IDisposable
{
  /// <summary>
  /// Gets a global variable from the script.
  /// </summary>
  public abstract Variant GetGlobal(string name);

  /// <summary>
  /// Sets a global variable in the script.
  /// </summary>
  public abstract void SetGlobal(string name, Variant value);

  /// <summary>
  /// Executes the script with the given arguments.
  /// </summary>
  public abstract Variant ExecuteFunction(string name, params Variant[] arguments);

  public virtual void Dispose()
  {
  }
}

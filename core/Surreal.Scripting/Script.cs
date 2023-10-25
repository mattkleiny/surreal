namespace Surreal.Scripting;

/// <summary>
/// A script asset, created from a <see cref="IScriptLanguage"/>.
/// </summary>
public abstract class Script : IDisposable
{
  /// <summary>
  /// Executes the script with the given arguments.
  /// </summary>
  public abstract Variant ExecuteFunction(string name, params Variant[] arguments);

  public virtual void Dispose()
  {
  }
}

using Surreal.Assets;

namespace Surreal.Scripting;

/// <summary>
/// A script asset, created from a <see cref="IScriptLanguage"/>.
/// </summary>
[AssetType("bbe7ef73-ed1c-4ba9-805a-9592362431d3")]
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

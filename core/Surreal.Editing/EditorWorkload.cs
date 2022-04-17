namespace Surreal;

/// <summary>Enables a workload in the <see cref="EditorApplication{TGame}"/>.</summary>
public abstract class EditorWorkload
{
  /// <summary>The <see cref="IServiceModule"/> for this editor workload.</summary>
  public abstract IServiceModule Services { get; }
}

namespace Surreal;

/// <summary>Enables a workload in the <see cref="EditorApplication{TGame}"/>.</summary>
public abstract class EditorWorkload
{
  /// <summary>The <see cref="IServiceModule"/> for this editor workload.</summary>
  public abstract IServiceModule Services { get; }

  /// <summary>Ticks the workload by a single frame.</summary>
  public virtual void Tick()
  {
  }
}

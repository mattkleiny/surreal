namespace Surreal;

/// <summary>Different modes of persistence.</summary>
public enum PersistenceMode
{
  /// <summary>Transient storage (perhaps between level loads or for in-memory testing).</summary>
  Transient,
  /// <summary>Permanent storage (for persistence to disk for later reload).</summary>
  Permanent,
}

/// <summary>Context for a persistence operation.</summary>
public readonly record struct PersistenceContext
{
  public PersistenceMode Mode { get; init; } = PersistenceMode.Transient;
}

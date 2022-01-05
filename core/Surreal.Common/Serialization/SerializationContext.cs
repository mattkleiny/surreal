namespace Surreal.Serialization;

/// <summary>Different models of serialization.</summary>
public enum SerializationMode
{
  Transient,
  Persistent,
}

/// <summary>Context for serialization operations.</summary>
public readonly record struct SerializationContext
{
  public SerializationMode Mode { get; init; }
}

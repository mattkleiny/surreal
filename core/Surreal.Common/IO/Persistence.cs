using Surreal.Collections;

namespace Surreal.IO;

/// <summary>Different modes of persistence.</summary>
public enum PersistenceMode
{
  /// <summary>Transient storage (perhaps between level loads or for in-memory testing).</summary>
  Transient,

  /// <summary>Permanent storage (for persistence to disk for later reload).</summary>
  Permanent,
}

/// <summary>Context for a persistence operation.</summary>
public readonly record struct PersistenceContext(
  IPersistenceStore Store,
  PersistenceMode Mode = PersistenceMode.Transient
);

/// <summary>Identifies an object with persistent data.</summary>
public interface IPersistentObject
{
  Guid Id { get; }

  void OnPersistState(PersistenceContext context, IPersistenceWriter writer);
  void OnResumeState(PersistenceContext context, IPersistenceReader reader);
}

/// <summary>Static extensions for <see cref="IPersistentObject"/>s.</summary>
public static class PersistentObjectExtensions
{
  public static void Persist(this IPersistentObject persistent, PersistenceContext context)
  {
    var writer = context.Store.CreateWriter(persistent.Id);

    persistent.OnPersistState(context, writer);
  }

  public static void Resume(this IPersistentObject persistent, PersistenceContext context)
  {
    if (context.Store.CreateReader(persistent.Id, out var reader))
    {
      persistent.OnResumeState(context, reader);
    }
  }
}

/// <summary>A store for persistent data.</summary>
public interface IPersistenceStore
{
  bool CreateReader(Guid identifier, out IPersistenceReader reader);
  IPersistenceWriter CreateWriter(Guid identifier);
}

/// <summary>A reader for persistent <see cref="Property{T}"/>s.</summary>
public interface IPersistenceReader
{
  T? Read<T>(Property<T> property, Optional<T> defaultValue = default);
}

/// <summary>A writer for persistent <see cref="Property{T}"/>s.</summary>
public interface IPersistenceWriter
{
  void Write<T>(Property<T> property, T value);
}

/// <summary>In-memory storage of persistent data, for use in testing and in-process transitions.</summary>
public sealed class InMemoryPersistenceStore : IPersistenceStore
{
  private readonly ConcurrentDictionary<Guid, PropertyBag> bagsById = new();

  public bool CreateReader(Guid identifier, out IPersistenceReader reader)
  {
    if (bagsById.TryGetValue(identifier, out var propertyBag))
    {
      reader = propertyBag;
      return true;
    }

    reader = default!;
    return false;
  }

  public IPersistenceWriter CreateWriter(Guid identifier)
  {
    return bagsById.GetOrAdd(identifier, _ => new PropertyBag());
  }

  /// <summary>A bag of properties for persistent read/writes.</summary>
  private sealed class PropertyBag : IPersistenceReader, IPersistenceWriter
  {
    private readonly Dictionary<string, object?> entries = new();

    public T? Read<T>(Property<T> property, Optional<T> defaultValue = default)
    {
      if (!entries.TryGetValue(property.Key, out var value))
      {
        return defaultValue.GetOrDefault(property.DefaultValue!);
      }

      return (T?) value;
    }

    public void Write<T>(Property<T> property, T value)
    {
      entries[property.Key] = value;
    }
  }
}

namespace Surreal.IO.Persistence;

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

    public T? Read<T>(PersistentProperty<T> property, Optional<T> defaultValue = default)
    {
      if (!entries.TryGetValue(property.Name, out var value))
      {
        return defaultValue.GetOrDefault(property.DefaultValue!);
      }

      return (T?) value;
    }

    public void Write<T>(PersistentProperty<T> property, T value)
    {
      entries[property.Name] = value;
    }
  }
}

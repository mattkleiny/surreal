namespace Surreal.Collections;

/// <summary>Identifies a single property by name, with a default value.</summary>
public readonly record struct Property<T>(string Key, T DefaultValue = default!)
{
  public override int GetHashCode()
  {
    return string.GetHashCode(Key, StringComparison.Ordinal);
  }

  public bool Equals(Property<T> other)
  {
    return string.Equals(Key, other.Key, StringComparison.Ordinal);
  }
}

/// <summary>A collection of <see cref="Property{T}"/>; a dictionary of structured types.</summary>
public interface IPropertyCollection
{
  T Get<T>(Property<T> property, Optional<T> defaultValue = default);
  void Set<T>(Property<T> property, T value);
  void Clear<T>(Property<T> property);
  void ClearAll();
}

/// <summary>The default <see cref="IPropertyCollection"/> implementation.</summary>
public sealed class PropertyCollection : IPropertyCollection
{
  private readonly Dictionary<Type, object> storagesByType = new();

  public T Get<T>(Property<T> property, Optional<T> defaultValue = default)
  {
    if (TryGetStorage<T>(out var storage) && storage.TryGetValue(property.Key, out var value))
    {
      return value;
    }

    return defaultValue.GetOrDefault(property.DefaultValue);
  }

  public void Set<T>(Property<T> property, T value)
  {
    var storage = GetOrCreateStorage<T>();

    storage[property.Key] = value;
  }

  public void Clear<T>(Property<T> property)
  {
    if (TryGetStorage<T>(out var storage))
    {
      storage.Remove(property.Key);
    }
  }

  public void ClearAll()
  {
    storagesByType.Clear();
  }

  private Dictionary<string, T> GetOrCreateStorage<T>()
  {
    if (!TryGetStorage<T>(out var storage))
    {
      storagesByType[typeof(T)] = storage = new Dictionary<string, T>(0, StringComparer.Ordinal);
    }

    return storage;
  }

  private bool TryGetStorage<T>(out Dictionary<string, T> result)
  {
    if (storagesByType.TryGetValue(typeof(T), out var dictionary))
    {
      result = (Dictionary<string, T>) dictionary;
      return true;
    }

    result = default!;
    return false;
  }
}

/// <summary>Static extensions for <see cref="IPropertyCollection"/> </summary>
public static class PropertyCollectionExtensions
{
  public static void Increment(this IPropertyCollection collection, Property<int> property, int amount = 1)
    => collection.Set(property, collection.Get(property) + amount);

  public static void Decrement(this IPropertyCollection collection, Property<int> property, int amount = 1)
    => collection.Set(property, collection.Get(property) - amount);
}

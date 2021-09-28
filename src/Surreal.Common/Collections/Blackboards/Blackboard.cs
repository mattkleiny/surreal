using System;
using System.Collections.Generic;

namespace Surreal.Collections.Blackboards
{
  /// <summary>The default <see cref="IBlackboard"/> implementation.</summary>
  public sealed class Blackboard : IBlackboard, IBlackboardSource
  {
    IBlackboard IBlackboardSource.Blackboard => this;

    private readonly Dictionary<Type, object> storageByKey = new();

    public T Get<T>(BlackboardProperty<T> property, Optional<T> defaultValue = default)
    {
      if (TryGetStorage<T>(out var storage) && storage.TryGetValue(property.Key, out var value))
      {
        return value;
      }

      return defaultValue.GetOrDefault(property.DefaultValue)!;
    }

    public void Set<T>(BlackboardProperty<T> property, T value)
    {
      var storage = GetOrCreateStorage<T>();

      storage[property.Key] = value;
    }

    public void Remove<T>(BlackboardProperty<T> property)
    {
      if (TryGetStorage<T>(out var storage))
      {
        storage.Remove(property.Key);
      }
    }

    public void Clear()
    {
      storageByKey.Clear();
    }

    private Dictionary<string, T> GetOrCreateStorage<T>()
    {
      if (!TryGetStorage<T>(out var storage))
      {
        storageByKey[typeof(T)] = storage = new Dictionary<string, T>(0, StringComparer.OrdinalIgnoreCase);
      }

      return storage;
    }

    private bool TryGetStorage<T>(out Dictionary<string, T> result)
    {
      if (storageByKey.TryGetValue(typeof(T), out var dictionary))
      {
        result = (Dictionary<string, T>) dictionary;
        return true;
      }

      result = default!;
      return false;
    }
  }
}

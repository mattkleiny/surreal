using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Surreal.Framework.Scenes.Entities.Components;

namespace Surreal.Framework.Scenes.Entities.Storage
{
  public sealed class SparseComponentStorage<T> : IComponentStorage<T>
    where T : IComponent
  {
    private readonly Dictionary<EntityId, Box> components;

    public SparseComponentStorage(int initialCapacity = 10)
    {
      components = new Dictionary<EntityId, Box>(initialCapacity);
    }

    public ref T Create(EntityId id, T instance)
    {
      var box = new Box(instance);

      components[id] = box;

      return ref box.Instance;
    }

    public ref T Get(EntityId id)
    {
      if (!components.TryGetValue(id, out var holder))
      {
        throw new Exception($"The given entity {id} does not contain a component of type {typeof(T)}");
      }

      return ref holder.Instance;
    }

    public bool Has(EntityId id)
    {
      return components.ContainsKey(id);
    }

    public void Remove(EntityId id)
    {
      components.Remove(id);
    }

    public void Cull(ReadOnlySpan<EntityId> entityIds)
    {
      for (var i = 0; i < entityIds.Length; i++)
      {
        Remove(entityIds[i]);
      }
    }

    public IEnumerator<T>   GetEnumerator() => components.Values.Select(holder => holder.Instance).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class Box
    {
      public Box(T instance)
      {
        Instance = instance;
      }

      public T Instance;
    }
  }
}
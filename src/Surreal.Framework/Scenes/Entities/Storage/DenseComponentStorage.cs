using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Surreal.Collections;
using Surreal.Framework.Scenes.Entities.Components;

namespace Surreal.Framework.Scenes.Entities.Storage {
  // TODO: make this aware of the entity's generation

  public sealed class DenseComponentStorage<T> : IComponentStorage<T>
      where T : IComponent {
    private readonly GrowingBitArray mask = new GrowingBitArray();
    private readonly Bag<T>          components;

    public DenseComponentStorage(int initialCapacity = 10) {
      Debug.Assert(initialCapacity > 0, "initialCapacity > 0");

      components = new Bag<T>(initialCapacity);
    }

    public ref T Create(EntityId id, T instance) {
      mask[id.Index]       = true;
      components[id.Index] = instance;

      return ref components.Get(id.Index);
    }

    public ref T Get(EntityId id) {
      return ref components.Get(id.Index);
    }

    public bool Has(EntityId id) {
      return mask[id.Index];
    }

    public void Remove(EntityId id) {
      if (Has(id)) {
        components.Remove(id.Index);
        mask[id.Index] = false;
      }
    }

    public void Cull(ReadOnlySpan<EntityId> entityIds) {
      for (var i = 0; i < entityIds.Length; i++) {
        Remove(entityIds[i]);
      }
    }

    public IEnumerator<T>   GetEnumerator() => components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Surreal.Collections;
using Surreal.Framework.Scenes.Entities.Storage;

namespace Surreal.Framework.Scenes.Entities.Components {
  internal sealed class ComponentManager {
    private readonly Bag<IComponentStorage> storageById;

    public ComponentManager(int maxComponentTypes) {
      storageById = new Bag<IComponentStorage>(maxComponentTypes);
    }

    public void RegisterComponent<T>(IComponentStorage<T> storage)
        where T : IComponent => storageById[ComponentType.Holder<T>.Instance.Id] = storage;

    public ref T CreateComponent<T>(EntityId entityId)
        where T : IComponent, new() => ref GetStorage<T>().Create(entityId, new T());

    public ref T CreateComponent<T>(EntityId entityId, T instance)
        where T : IComponent => ref GetStorage<T>().Create(entityId, instance);

    public ref T GetComponent<T>(EntityId entityId)
        where T : IComponent => ref GetStorage<T>().Get(entityId);

    public bool HasComponent<T>(EntityId entityId)
        where T : IComponent => GetStorage<T>().Has(entityId);

    public void RemoveComponent<T>(EntityId entityId)
        where T : IComponent => GetStorage<T>().Remove(entityId);

    public IComponentMapper<T> GetComponentMapper<T>()
        where T : IComponent, new() => new ComponentMapper<T>(GetStorage<T>());

    public void Cull(ReadOnlySpan<EntityId> entityIds) {
      for (var i = 0; i < storageById.Count; i++) {
        storageById[i].Cull(entityIds);
      }
    }

    public void DisposeAndClear() {
      foreach (var storage in storageById.OfType<IDisposable>()) {
        storage.Dispose();
      }

      storageById.Clear();
    }

    private IComponentStorage<T> GetStorage<T>()
        where T : IComponent {
      var componentType = ComponentType.Holder<T>.Instance;
      var storage       = storageById[componentType.Id];

      if (storage == null) {
        throw new UnregisteredComponentException($"The given component type is not registered with the system: {typeof(T)}");
      }

      return (IComponentStorage<T>) storage;
    }

    private sealed class ComponentMapper<T> : IComponentMapper<T>
        where T : IComponent, new() {
      private readonly IComponentStorage<T> storage;

      public ComponentMapper(IComponentStorage<T> storage) {
        this.storage = storage;
      }

      public ref T    Create(EntityId id)             => ref storage.Create(id, new T());
      public ref T    Create(EntityId id, T instance) => ref storage.Create(id, instance);
      public ref T    Get(EntityId id)                => ref storage.Get(id);
      public     bool Has(EntityId id)                => storage.Has(id);
      public     void Remove(EntityId id)             => storage.Remove(id);

      public IEnumerator<T>   GetEnumerator() => storage.GetEnumerator();
      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
  }
}
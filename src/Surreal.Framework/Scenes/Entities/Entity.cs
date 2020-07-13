using System;
using System.Diagnostics;
using Surreal.Framework.Scenes.Entities.Components;

namespace Surreal.Framework.Scenes.Entities {
  [DebuggerDisplay("Entity (Id={Id})")]
  public readonly struct Entity : IEquatable<Entity> {
    public readonly EntityId    Id;
    public readonly EntityScene Scene;

    public Entity(EntityId id, EntityScene scene) {
      Id    = id;
      Scene = scene;
    }

    public bool Exists  => Scene.EntityManager.Exists(Id);
    public bool IsDead  => !Exists;
    public bool IsDying => Scene.EntityManager.IsQueuedForRemoval(Id);

    public ref T Create<T>(T instance)
        where T : IComponent, new() => ref Scene.ComponentManager.CreateComponent(Id, instance);

    public ref T Create<T>()
        where T : IComponent, new() => ref Scene.ComponentManager.CreateComponent<T>(Id);

    public ref T Get<T>()
        where T : IComponent => ref Scene.ComponentManager.GetComponent<T>(Id);

    public bool Has<T>()
        where T : IComponent => Scene.ComponentManager.HasComponent<T>(Id);

    public void Remove<T>()
        where T : IComponent => Scene.ComponentManager.RemoveComponent<T>(Id);

    public void Delete() => Scene.DeleteEntity(Id);

    public bool Equals(Entity other) {
      return Id.Equals(other.Id);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;

      return obj is Entity other && Equals(other);
    }

    public override int    GetHashCode() => Id.GetHashCode();
    public override string ToString()    => $"<Entity {Id}>";
  }
}
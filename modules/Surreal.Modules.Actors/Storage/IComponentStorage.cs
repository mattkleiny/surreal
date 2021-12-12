namespace Surreal.Storage;

/// <summary>Represents generically any <see cref="IComponentStorage{T}"/> type.</summary>
public interface IComponentStorage
{
  void Prune(ActorId id);
}

/// <summary>Represents storage for a component of type <see cref="T"/>.</summary>
public interface IComponentStorage<T> : IComponentStorage
{
  ref T GetComponent(ActorId id);
  ref T AddComponent(ActorId id, Optional<T> prototype);
  bool  RemoveComponent(ActorId id);
}

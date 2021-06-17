namespace Surreal.Modules.Actors.Components {
  public interface IComponentStorage<T> {
    bool TryGetComponent(ActorId id, out T component);
    T    AddComponent(ActorId id, Optional<T> prototype);
    bool RemoveComponent(ActorId id);
  }
}
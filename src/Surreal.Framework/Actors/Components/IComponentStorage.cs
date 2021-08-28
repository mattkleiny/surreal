namespace Surreal.Actors.Components
{
  public interface IComponentStorage
  {
    void Prune(ActorId id);
  }

  public interface IComponentStorage<T> : IComponentStorage
  {
    ref T GetComponent(ActorId id);
    ref T AddComponent(ActorId id, Optional<T> prototype);
    bool  RemoveComponent(ActorId id);
  }
}

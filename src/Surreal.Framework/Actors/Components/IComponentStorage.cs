namespace Surreal.Framework.Actors.Components
{
  public interface IComponentStorage<T>
  {
    ref T GetComponent(ActorId id);
    ref T AddComponent(ActorId id, Optional<T> prototype);
    bool  RemoveComponent(ActorId id);
  }
}

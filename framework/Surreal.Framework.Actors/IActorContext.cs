using Surreal.Framework.Components;

namespace Surreal.Framework
{
  public interface IActorContext
  {
    ActorId     AllocateId();
    ActorStatus GetStatus(ActorId id);

    void Spawn(Actor actor);
    void Enable(ActorId id);
    void Disable(ActorId id);
    void Destroy(ActorId id);

    IComponentStorage<T> GetStorage<T>();
  }
}
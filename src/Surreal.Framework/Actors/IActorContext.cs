using Surreal.Framework.Actors.Components;

namespace Surreal.Framework.Actors
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
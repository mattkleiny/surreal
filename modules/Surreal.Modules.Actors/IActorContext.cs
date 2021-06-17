using Surreal.Modules.Actors.Components;

namespace Surreal.Modules.Actors {
  public interface IActorContext {
    ActorId AllocateId();
    void    QueueDestroy(ActorId id);

    IComponentStorage<T> GetStorage<T>();
  }
}
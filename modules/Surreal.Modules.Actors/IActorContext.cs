using Surreal.Components;

namespace Surreal;

/// <summary>The context in which an <see cref="Actor"/> lives; hooked up after spawning into a scene.</summary>
public interface IActorContext
{
  ActorStatus GetStatus(ActorId id);

  void Spawn(Actor actor);

  void Enable(ActorId id);
  void Disable(ActorId id);
  void Destroy(ActorId id);

  IComponentStorage<T> GetStorage<T>()
    where T : notnull;
}

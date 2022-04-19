using Surreal.Aspects;
using Surreal.Components;

namespace Surreal;

/// <summary>The context in which an <see cref="Actor"/> lives; hooked up after spawning into a scene.</summary>
internal interface IActorContext
{
  IServiceProvider? Services { get; }

  ActorStatus GetStatus(ActorId id);

  ActorId AllocateId();

  void Enable(ActorId id);
  void Disable(ActorId id);
  void Destroy(ActorId id);

  IComponentStorage<T> GetStorage<T>()
    where T : notnull, new();

  void RemoveSubscription(AspectSubscription subscription);
}

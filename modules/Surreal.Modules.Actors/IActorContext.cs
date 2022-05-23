using Surreal.Collections;

namespace Surreal;

/// <summary>The context in which an <see cref="Actor"/> lives; hooked up after spawning into a scene.</summary>
internal interface IActorContext
{
  IServiceProvider? Services { get; }

  ActorStatus GetStatus(ArenaIndex index);

  T Spawn<T>(T actor)
    where T : Actor;

  void Enable(ArenaIndex index);
  void Disable(ArenaIndex index);
  void Destroy(ArenaIndex index);
}

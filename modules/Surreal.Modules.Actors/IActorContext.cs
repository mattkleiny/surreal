using Surreal.Collections;

namespace Surreal;

/// <summary>The context in which an <see cref="Actor"/> lives; hooked up after spawning into a scene.</summary>
internal interface IActorContext
{
  IServiceProvider? Services { get; }

  ActorStatus GetStatus(ArenaIndex id);

  T Spawn<T>(T actor)
    where T : Actor;

  void Enable(ArenaIndex id);
  void Disable(ArenaIndex id);
  void Destroy(ArenaIndex id);
}

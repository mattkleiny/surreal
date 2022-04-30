namespace Surreal.Actors;

/// <summary>The context in which an <see cref="Actor"/> lives; hooked up after spawning into a scene.</summary>
internal interface IActorContext
{
  IServiceProvider? Services { get; }

  ActorStatus GetStatus(ActorId id);

  T Spawn<T>(T actor)
    where T : Actor;

  ActorId AllocateId();

  void Enable(ActorId id);
  void Disable(ActorId id);
  void Destroy(ActorId id);
}

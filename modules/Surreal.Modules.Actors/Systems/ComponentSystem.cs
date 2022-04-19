using Surreal.Aspects;
using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>Base class for any <see cref="SceneSystem"/> implementation that monitors components..</summary>
public abstract class ComponentSystem : IDisposable
{
  private readonly Aspect aspect;
  private AspectSubscription? subscription;

  protected ComponentSystem(Aspect aspect)
  {
    this.aspect = aspect;
  }

  public HashSet<ActorId> ActorIds { get; } = new();
  public ActorScene?      Scene    { get; private set; }

  public virtual void OnAddedToScene(ActorScene scene)
  {
    Scene        = scene;
    subscription = scene.SubscribeToAspect(aspect);

    subscription.Added   += OnActorAdded;
    subscription.Removed += OnActorRemoved;
  }

  public virtual void OnRemovedFromScene(ActorScene scene)
  {
    if (subscription != null)
    {
      subscription.Added   -= OnActorAdded;
      subscription.Removed -= OnActorRemoved;

      subscription.Dispose();
    }

    Scene = null;
  }

  public virtual void OnBeginFrame(DeltaTime deltaTime)
  {
  }

  public virtual void OnInput(DeltaTime deltaTime)
  {
  }

  public virtual void OnUpdate(DeltaTime deltaTime)
  {
  }

  public virtual void OnDraw(DeltaTime deltaTime)
  {
  }

  public virtual void OnEndFrame(DeltaTime deltaTime)
  {
  }

  public virtual void Dispose()
  {
  }

  private void OnActorAdded(ActorId id) => ActorIds.Add(id);
  private void OnActorRemoved(ActorId id) => ActorIds.Remove(id);
}

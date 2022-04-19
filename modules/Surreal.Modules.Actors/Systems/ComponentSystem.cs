using Surreal.Aspects;

namespace Surreal.Systems;

/// <summary>Base class for any <see cref="ISceneSystem"/> implementation that monitors components..</summary>
public abstract class ComponentSystem : SceneSystem
{
  private readonly Aspect aspect;
  private AspectSubscription? subscription;

  protected ComponentSystem(Aspect aspect)
  {
    this.aspect = aspect;
  }

  public override void OnAddedToScene(ActorScene scene)
  {
    base.OnAddedToScene(scene);

    subscription = scene.SubscribeToAspect(aspect);

    subscription.Added   += OnActorAdded;
    subscription.Removed += OnActorRemoved;
  }

  public override void OnRemovedFromScene(ActorScene scene)
  {
    if (subscription != null)
    {
      subscription.Added   -= OnActorAdded;
      subscription.Removed -= OnActorRemoved;

      subscription.Dispose();
    }

    base.OnRemovedFromScene(scene);
  }

  private void OnActorAdded(ActorId id)
  {
  }

  private void OnActorRemoved(ActorId id)
  {
  }
}

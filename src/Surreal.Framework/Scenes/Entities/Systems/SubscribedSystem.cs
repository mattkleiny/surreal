using System;
using Surreal.Framework.Scenes.Entities.Aspects;

namespace Surreal.Framework.Scenes.Entities.Systems
{
  public abstract class SubscribedSystem : EntitySystem
  {
    private readonly Aspect aspect;

    protected SubscribedSystem(Aspect aspect)
    {
      this.aspect = aspect;
    }

    protected ReadOnlySpan<EntityId> Entities
    {
      get
      {
        if (Subscription == null)
        {
          return ReadOnlySpan<EntityId>.Empty;
        }

        return Subscription.Entities;
      }
    }

    protected IAspectSubscription? Subscription { get; private set; }

    public override void Initialize(EntityScene scene)
    {
      base.Initialize(scene);

      Subscription = scene.Subscribe(aspect);

      Subscription.EntityAdded   += OnEntityAdded;
      Subscription.EntityRemoved += OnEntityRemoved;
    }

    protected virtual void OnEntityAdded(Entity entity)
    {
    }

    protected virtual void OnEntityRemoved(Entity entity)
    {
    }

    public override void Dispose()
    {
      if (Subscription != null)
      {
        Subscription.EntityAdded   -= OnEntityAdded;
        Subscription.EntityRemoved -= OnEntityRemoved;
      }

      base.Dispose();
    }

    private void OnEntityAdded(EntityId id)
    {
      if (World != null)
      {
        OnEntityAdded(World.GetEntity(id));
      }
    }

    private void OnEntityRemoved(EntityId id)
    {
      if (World != null)
      {
        OnEntityRemoved(World.GetEntity(id));
      }
    }
  }
}
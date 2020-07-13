using System;
using System.Collections.Generic;
using System.Diagnostics;
using Surreal.Collections;

namespace Surreal.Framework.Scenes.Entities.Aspects {
  internal sealed class AspectManager {
    private readonly Dictionary<Aspect, AspectSubscription> subscriptions = new Dictionary<Aspect, AspectSubscription>();

    public IAspectSubscription Subscribe(Aspect aspect) {
      return subscriptions.GetOrCompute(aspect, _ => new AspectSubscription(_));
    }

    public void Refresh(ReadOnlySpan<EntityId> addedEntities, ReadOnlySpan<EntityId> removedEntities) {
      foreach (var subscription in subscriptions.Values) {
        subscription.Update(addedEntities, removedEntities);
      }
    }

    [DebuggerDisplay("Subscription for {Aspect}")]
    private sealed class AspectSubscription : IAspectSubscription {
      private readonly Bag<EntityId> entities = new Bag<EntityId>();

      public AspectSubscription(Aspect aspect) {
        Aspect = aspect;
      }

      public event Action<EntityId> EntityAdded;
      public event Action<EntityId> EntityRemoved;

      public Aspect Aspect { get; }

      ReadOnlySpan<EntityId> IAspectSubscription.Entities => entities.Span;

      public void Update(ReadOnlySpan<EntityId> addedEntities, ReadOnlySpan<EntityId> removedEntities) {
        // TODO: honor the aspect here
        foreach (var entity in addedEntities) {
          entities.Add(entity);

          EntityAdded?.Invoke(entity);
        }

        foreach (var entity in removedEntities) {
          entities.Remove(entity);

          EntityRemoved?.Invoke(entity);
        }
      }
    }
  }
}
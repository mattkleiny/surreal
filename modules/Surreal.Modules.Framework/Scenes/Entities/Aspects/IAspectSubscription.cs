using System;

namespace Surreal.Framework.Scenes.Entities.Aspects {
  public interface IAspectSubscription {
    event Action<EntityId> EntityAdded;
    event Action<EntityId> EntityRemoved;

    Aspect                 Aspect   { get; }
    ReadOnlySpan<EntityId> Entities { get; }
  }
}
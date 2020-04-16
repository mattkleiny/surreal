using System;
using System.Collections.Generic;
using Surreal.Framework.Scenes.Entities.Collections;

namespace Surreal.Framework.Scenes.Entities.Systems
{
  internal sealed class EntityManager
  {
    private readonly List<EntityId>  queuedToAdd;
    private readonly List<EntityId>  queuedToRemove;
    private readonly SlotMap<object> entities;

    public EntityManager(int maxEntityCount)
    {
      queuedToAdd    = new List<EntityId>(maxEntityCount);
      queuedToRemove = new List<EntityId>(maxEntityCount);
      entities       = new SlotMap<object>(maxEntityCount);
    }

    public EntityId Create()
    {
      var entityId = new EntityId(entities.Add(null!));

      queuedToAdd.Add(entityId);

      return entityId;
    }

    public bool Exists(EntityId entityId)
    {
      return entities.Contains(entityId.Key);
    }

    public void Remove(EntityId entityId)
    {
      queuedToRemove.Add(entityId);
    }

    public bool IsQueuedForRemoval(EntityId entityId)
    {
      return queuedToRemove.Contains(entityId);
    }

    public bool Flush(out ReadOnlySpan<EntityId> addedEntities, out ReadOnlySpan<EntityId> removedEntities)
    {
      if (queuedToAdd.Count > 0 || queuedToRemove.Count > 0)
      {
        addedEntities   = queuedToAdd.ToArray();
        removedEntities = queuedToRemove.ToArray();

        for (var i = 0; i < removedEntities.Length; i++)
        {
          entities.TryRemove(removedEntities[i].Key);
        }

        queuedToAdd.Clear();
        queuedToRemove.Clear();

        return true;
      }

      addedEntities   = Span<EntityId>.Empty;
      removedEntities = Span<EntityId>.Empty;

      return false;
    }
  }
}
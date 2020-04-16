using System;
using System.Collections;
using System.Collections.Generic;
using Surreal.Framework.Scenes.Entities.Components;

namespace Surreal.Framework.Scenes.Entities.Storage
{
  public interface IComponentStorage : IEnumerable
  {
    void Cull(ReadOnlySpan<EntityId> entityIds);
  }

  public interface IComponentStorage<T> : IComponentStorage, IEnumerable<T>
    where T : IComponent
  {
    ref T Create(EntityId id, T instance);
    ref T Get(EntityId id);
    bool  Has(EntityId id);
    void  Remove(EntityId id);
  }
}
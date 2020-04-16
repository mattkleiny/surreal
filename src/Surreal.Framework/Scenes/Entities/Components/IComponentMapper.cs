using System.Collections.Generic;

namespace Surreal.Framework.Scenes.Entities.Components
{
  public interface IComponentMapper<T> : IEnumerable<T>
    where T : IComponent
  {
    ref T Create(EntityId id);
    ref T Create(EntityId id, T instance);
    ref T Get(EntityId id);
    bool  Has(EntityId id);
    void  Remove(EntityId id);
  }
}

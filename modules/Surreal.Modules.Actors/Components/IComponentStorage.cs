using System.Runtime.CompilerServices;
using Surreal.Collections;

namespace Surreal.Components;

/// <summary>Represents generically any <see cref="IComponentStorage{T}"/> type.</summary>
public interface IComponentStorage
{
  bool RemoveComponent(ArenaIndex id);
}

/// <summary>Represents storage for a component of type <see cref="T"/>.</summary>
public interface IComponentStorage<T> : IComponentStorage
  where T : notnull
{
  ref T GetOrCreateComponent(ArenaIndex id, Optional<T> prototype)
  {
    ref var component = ref GetComponent(id);

    if (Unsafe.IsNullRef(ref component))
    {
      return ref AddComponent(id, prototype);
    }

    return ref component;
  }

  ref T GetComponent(ArenaIndex id);
  ref T AddComponent(ArenaIndex id, Optional<T> prototype);
}

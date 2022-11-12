using System.Runtime.CompilerServices;
using Surreal.Collections;

namespace Surreal.Actors.Components;

/// <summary>Represents generically any <see cref="IComponentStorage{T}" /> type.</summary>
public interface IComponentStorage
{
  void RemoveComponent(ArenaIndex index);
}

/// <summary>Represents storage for a component of type <see cref="T" />.</summary>
public interface IComponentStorage<T> : IComponentStorage
  where T : notnull
{
  ref T GetOrCreateComponent(ArenaIndex index, Optional<T> prototype)
  {
#pragma warning disable CS8619
    ref var component = ref GetComponent(index);

    if (Unsafe.IsNullRef(ref component))
    {
      return ref AddComponent(index, prototype);
    }

    return ref component;
#pragma warning restore CS8619
  }

  ref T GetComponent(ArenaIndex index);
  ref T AddComponent(ArenaIndex index, Optional<T> prototype);
}

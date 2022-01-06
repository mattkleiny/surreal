﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Surreal.Components;

/// <summary>Represents generically any <see cref="IComponentStorage{T}"/> type.</summary>
public interface IComponentStorage
{
  bool UnsafeTryGetComponent(ActorId id, [NotNullWhen(true)] out object? component);
  void UnsafeAddComponent(ActorId id, object component);
}

/// <summary>Represents storage for a component of type <see cref="T"/>.</summary>
public interface IComponentStorage<T> : IComponentStorage
  where T : notnull
{
  ref T GetOrCreateComponent(ActorId id, Optional<T> prototype)
  {
    ref var component = ref GetComponent(id);

    if (Unsafe.IsNullRef(ref component))
    {
      return ref AddComponent(id, prototype);
    }

    return ref component;
  }

  ref T GetComponent(ActorId id);
  ref T AddComponent(ActorId id, Optional<T> prototype);
  bool  RemoveComponent(ActorId id);
}

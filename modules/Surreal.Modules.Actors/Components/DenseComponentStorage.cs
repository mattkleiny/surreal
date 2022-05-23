﻿using Surreal.Collections;

namespace Surreal.Components;

/// <summary>A densely packed <see cref="IComponentStorage{T}"/>.</summary>
public sealed class DenseComponentStorage<T> : IComponentStorage<T>
  where T : notnull, new()
{
  public ref T GetOrCreateComponent(ArenaIndex id, Optional<T> prototype)
  {
    throw new NotImplementedException();
  }

  public ref T GetComponent(ArenaIndex id)
  {
    throw new NotImplementedException();
  }

  public ref T AddComponent(ArenaIndex id, Optional<T> prototype)
  {
    throw new NotImplementedException();
  }

  public bool RemoveComponent(ArenaIndex id)
  {
    throw new NotImplementedException();
  }
}

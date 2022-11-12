using Surreal.Collections;

namespace Surreal.Actors.Components;

/// <summary>A densely packed <see cref="IComponentStorage{T}" />.</summary>
public sealed class DenseComponentStorage<T> : IComponentStorage<T>
  where T : notnull, new()
{
  private readonly GenerationalArena<T> _components = new();

  public ref T GetComponent(ArenaIndex index)
  {
    return ref _components[index];
  }

  public ref T AddComponent(ArenaIndex index, Optional<T> prototype)
  {
    _components.Insert(index, prototype.GetOrDefault(new T()));

    return ref _components[index];
  }

  public void RemoveComponent(ArenaIndex index)
  {
    _components.Remove(index);
  }
}




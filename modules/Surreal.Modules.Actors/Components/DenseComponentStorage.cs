using Surreal.Collections;

namespace Surreal.Components;

/// <summary>A densely packed <see cref="IComponentStorage{T}"/>.</summary>
public sealed class DenseComponentStorage<T> : IComponentStorage<T>
  where T : notnull, new()
{
  private readonly GenerationalArena<T> components = new();

  public ref T GetComponent(ArenaIndex index)
  {
    return ref components[index];
  }

  public ref T AddComponent(ArenaIndex index, Optional<T> prototype)
  {
    components.Insert(index, prototype.GetOrDefault(new T()));

    return ref components[index];
  }

  public void RemoveComponent(ArenaIndex index)
  {
    components.Remove(index);
  }
}

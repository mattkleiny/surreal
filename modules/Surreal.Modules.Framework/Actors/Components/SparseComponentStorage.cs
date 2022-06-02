using Surreal.Collections;

namespace Surreal.Components;

/// <summary>A sparsely packed <see cref="IComponentStorage{T}"/>.</summary>
public sealed class SparseComponentStorage<T> : IComponentStorage<T>
  where T : notnull, new()
{
  private readonly Dictionary<ArenaIndex, T> components = new();

  public ref T GetComponent(ArenaIndex index)
  {
    return ref components.GetRef(index);
  }

  public ref T AddComponent(ArenaIndex index, Optional<T> prototype)
  {
    components[index] = prototype.GetOrDefault(new T());

    return ref components.GetRef(index);
  }

  public void RemoveComponent(ArenaIndex index)
  {
    components.Remove(index);
  }
}

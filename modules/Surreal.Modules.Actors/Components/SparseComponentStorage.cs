using System.Runtime.CompilerServices;

namespace Surreal.Components;

/// <summary>A sparsely packed <see cref="IComponentStorage{T}"/>.</summary>
public sealed class SparseComponentStorage<T> : IComponentStorage<T>
  where T : notnull, new()
{
  private readonly Dictionary<ActorId, Box<T>> boxes = new();

  public ref T GetOrCreateComponent(ActorId id, Optional<T> prototype)
  {
    if (!boxes.TryGetValue(id, out var slot))
    {
      boxes[id] = slot = new Box<T>(prototype.GetOrDefault(new()));
    }

    return ref slot.Value;
  }

  public ref T GetComponent(ActorId id)
  {
    if (boxes.TryGetValue(id, out var slot))
    {
      return ref slot.Value;
    }

    return ref Unsafe.NullRef<T>();
  }

  public ref T AddComponent(ActorId id, Optional<T> prototype)
  {
    var component = prototype.GetOrDefault(new());
    var box       = new Box<T>(component);

    boxes[id] = box;

    return ref box.Value;
  }

  public bool RemoveComponent(ActorId id)
  {
    return boxes.Remove(id);
  }
}

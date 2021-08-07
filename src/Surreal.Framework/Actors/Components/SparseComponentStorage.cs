using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Surreal.Framework.Actors.Components
{
  public sealed class SparseComponentStorage<T> : IComponentStorage<T>
  {
    private readonly Dictionary<ActorId, Box<T>> slots = new();

    public ref T GetComponent(ActorId id)
    {
      if (slots.TryGetValue(id, out var slot))
      {
        return ref slot.Value;
      }

      return ref Unsafe.NullRef<T>();
    }

    public ref T AddComponent(ActorId id, Optional<T> prototype)
    {
      var component = prototype.GetOrDefault()!;
      var slot      = new Box<T>(component);

      slots[id] = slot;

      return ref slot.Value;
    }

    public bool RemoveComponent(ActorId id)
    {
      return slots.Remove(id);
    }

    public void Prune(ActorId id)
    {
      slots.Remove(id);
    }
  }
}

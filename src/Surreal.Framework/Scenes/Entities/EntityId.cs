using System;
using Surreal.Framework.Scenes.Entities.Collections;

namespace Surreal.Framework.Scenes.Entities
{
  public readonly struct EntityId : IEquatable<EntityId>
  {
    internal readonly SlotMapKey Key;

    internal EntityId(SlotMapKey key)
    {
      Key = key;
    }

    internal int Index => Key.Index;

    public bool Equals(EntityId other)
    {
      return Key == other.Key;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;

      return obj is EntityId other && Equals(other);
    }

    public static bool operator ==(EntityId left, EntityId right) => left.Equals(right);
    public static bool operator !=(EntityId left, EntityId right) => !left.Equals(right);

    public override int    GetHashCode() => Key.GetHashCode();
    public override string ToString()    => $"{Key.Index}";
  }
}
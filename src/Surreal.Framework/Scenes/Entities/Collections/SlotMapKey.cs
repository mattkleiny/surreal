using System;

namespace Surreal.Framework.Scenes.Entities.Collections
{
  internal readonly struct SlotMapKey : IEquatable<SlotMapKey>
  {
    public SlotMapKey(int index, int version)
    {
      Index   = index;
      Version = version;
    }

    public void Deconstruct(out int index, out int version)
    {
      index   = Index;
      version = Version;
    }

    public readonly int Index;
    public readonly int Version;

    public bool Equals(SlotMapKey other)
    {
      return Index == other.Index && Version == other.Version;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;

      return obj is EntityId other && Equals(other);
    }

    public static bool operator ==(SlotMapKey left, SlotMapKey right) => left.Equals(right);
    public static bool operator !=(SlotMapKey left, SlotMapKey right) => !left.Equals(right);

    public override int GetHashCode() => HashCode.Combine(Index, Version);

    public override string ToString() => $"{Index}@{Version}";
  }
}

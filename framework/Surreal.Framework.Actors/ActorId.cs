using System;

namespace Surreal.Framework
{
  public readonly struct ActorId : IEquatable<ActorId>
  {
    public static ActorId None => default;

    public readonly uint Id;

    public ActorId(uint id)
    {
      Id = id;
    }

    public override string ToString() => $"<{Id.ToString()}>";

    public          bool Equals(ActorId other) => Id == other.Id;
    public override bool Equals(object? obj)   => obj is ActorId other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(ActorId left, ActorId right) => left.Equals(right);
    public static bool operator !=(ActorId left, ActorId right) => !left.Equals(right);
  }
}
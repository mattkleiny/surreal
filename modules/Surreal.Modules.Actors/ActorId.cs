using System;

namespace Surreal.Modules.Actors {
  public readonly struct ActorId : IEquatable<ActorId> {
    public ushort Id         { get; }
    public ushort Generation { get; }

    public ActorId(ushort id, ushort generation) {
      Id         = id;
      Generation = generation;
    }

    public          bool Equals(ActorId other) => Id == other.Id && Generation == other.Generation;
    public override bool Equals(object? obj)   => obj is ActorId other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Id, Generation);

    public static bool operator ==(ActorId left, ActorId right) => left.Equals(right);
    public static bool operator !=(ActorId left, ActorId right) => !left.Equals(right);
  }
}
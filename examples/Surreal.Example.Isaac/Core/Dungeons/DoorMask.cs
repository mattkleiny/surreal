using System;
using Surreal.Mathematics.Linear;
using Surreal.Utilities;

namespace Isaac.Core.Dungeons {
  public struct DoorMask : IEquatable<DoorMask> {
    public static DoorMask None => default;
    public static DoorMask All  => new DoorMask(Direction.All);

    private Direction doors;

    public DoorMask(Direction doors) {
      this.doors = doors;
    }

    public bool HasDoor(Direction direction)    => doors.HasFlagFast(direction);
    public void AddDoor(Direction direction)    => doors |= direction;
    public void RemoveDoor(Direction direction) => doors &= ~direction;

    public override string ToString() => $"Doors {doors.ToPermutationString()}";

    public          bool Equals(DoorMask other) => doors == other.doors;
    public override bool Equals(object? obj)    => obj is DoorMask other && Equals(other);

    public override int GetHashCode() => (int) doors;

    public static bool operator ==(DoorMask left, DoorMask right) => left.Equals(right);
    public static bool operator !=(DoorMask left, DoorMask right) => !left.Equals(right);

    public static implicit operator DoorMask(Direction direction) => new DoorMask(direction);
  }
}
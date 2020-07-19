using System;
using Surreal.Mathematics.Linear;
using Surreal.Utilities;

namespace Isaac.Core {
  public struct DoorMask : IEquatable<DoorMask> {
    public static DoorMask None => default;
    public static DoorMask All  => new DoorMask(Directions.All);

    private Directions doors;

    public DoorMask(Directions doors) {
      this.doors = doors;
    }

    public bool HasDoor(Directions directions)    => doors.HasFlagFast(directions);
    public void AddDoor(Directions directions)    => doors |= directions;
    public void RemoveDoor(Directions directions) => doors &= ~directions;

    public override string ToString() => $"Doors {doors.ToPermutationString()}";

    public          bool Equals(DoorMask other) => doors == other.doors;
    public override bool Equals(object? obj)    => obj is DoorMask other && Equals(other);

    public override int GetHashCode() => (int) doors;

    public static bool operator ==(DoorMask left, DoorMask right) => left.Equals(right);
    public static bool operator !=(DoorMask left, DoorMask right) => !left.Equals(right);
  }
}
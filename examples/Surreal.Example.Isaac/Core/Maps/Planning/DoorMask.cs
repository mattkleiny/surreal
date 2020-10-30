using System;
using System.Collections;
using System.Collections.Generic;
using Surreal.Mathematics.Linear;
using Surreal.Utilities;

namespace Isaac.Core.Maps.Planning {
  public struct DoorMask : IEquatable<DoorMask>, IEnumerable<Direction> {
    public static DoorMask None => default;
    public static DoorMask All  => new(Direction.All);

    private Direction doors;

    public DoorMask(Direction doors) {
      this.doors = doors;
    }

    public Direction UsedDoors => doors;
    public Direction FreeDoors => Direction.All & ~doors;

    public bool IsUsed(Direction direction) => UsedDoors.HasFlagFast(direction);
    public bool IsFree(Direction direction) => FreeDoors.HasFlagFast(direction);
    public void Add(Direction direction)    => doors |= direction;
    public void Remove(Direction direction) => doors &= ~direction;

    public override string ToString() => $"Doors {doors.ToPermutationString()}";

    public          bool Equals(DoorMask other) => doors == other.doors;
    public override bool Equals(object? obj)    => obj is DoorMask other && Equals(other);

    public override int GetHashCode() => (int) doors;

    public static bool operator ==(DoorMask left, DoorMask right) => left.Equals(right);
    public static bool operator !=(DoorMask left, DoorMask right) => !left.Equals(right);

    public static implicit operator DoorMask(Direction direction) => new(direction);

    public EnumExtensions.MaskEnumerator<Direction> GetEnumerator() => doors.GetMaskValues();
    IEnumerator<Direction> IEnumerable<Direction>.  GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                        GetEnumerator() => GetEnumerator();
  }
}
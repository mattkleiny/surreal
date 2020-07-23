using System;
using System.Collections;
using System.Collections.Generic;
using Surreal.Mathematics.Linear;
using Surreal.Utilities;

namespace Isaac.Core.Maps.Planning {
  public struct DoorMask : IEquatable<DoorMask>, IEnumerable<Direction> {
    public static DoorMask None => default;
    public static DoorMask All  => new DoorMask(Direction.All);

    private Direction doors;

    public DoorMask(Direction doors) {
      this.doors = doors;
    }

    public Direction UnusedDoors => Direction.All & ~doors;

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


    public Enumerator                             GetEnumerator() => new Enumerator(doors);
    IEnumerator<Direction> IEnumerable<Direction>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                      GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<Direction> {
      private readonly Direction direction;

      public Enumerator(Direction direction) {
        this.direction = direction;

        Current = Direction.None;
      }

      public Direction    Current { get; }
      object? IEnumerator.Current => Current;

      public bool MoveNext() {
        throw new NotImplementedException();
      }

      public void Reset() {
        throw new NotImplementedException();
      }

      public void Dispose() {
        // no-op
      }
    }
  }
}
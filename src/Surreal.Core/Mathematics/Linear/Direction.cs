﻿using System;
using Surreal.Utilities;

namespace Surreal.Mathematics.Linear {
  [Flags]
  public enum Direction : byte {
    None  = 0,
    North = 1 << 1,
    East  = 1 << 2,
    South = 1 << 3,
    West  = 1 << 4,
    All   = North | East | South | West
  }

  public static class DirectionExtensions {
    public static Direction Opposite(this Direction direction) {
      var opposite = Direction.None;

      if (direction.HasFlagFast(Direction.North)) opposite |= Direction.South;
      if (direction.HasFlagFast(Direction.South)) opposite |= Direction.North;
      if (direction.HasFlagFast(Direction.East)) opposite  |= Direction.West;
      if (direction.HasFlagFast(Direction.West)) opposite  |= Direction.East;

      return opposite;
    }

    public static Vector2I ToVector2I(this Direction direction) {
      var vector = Vector2I.Zero;

      if (direction.HasFlagFast(Direction.North)) vector.Y += 1;
      if (direction.HasFlagFast(Direction.South)) vector.Y -= 1;
      if (direction.HasFlagFast(Direction.East)) vector.X  += 1;
      if (direction.HasFlagFast(Direction.West)) vector.X  -= 1;

      return vector;
    }
  }
}
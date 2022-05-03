using Surreal.Collections;

namespace Surreal.Mathematics;

/// <summary>Possible cardinal directions.</summary>
[Flags]
public enum Direction : byte
{
  None = 0,
  North = 1 << 1,
  East = 1 << 2,
  South = 1 << 3,
  West = 1 << 4,
  All = North | East | South | West
}

/// <summary>Extensions for <see cref="Direction"/> and co.</summary>
public static class DirectionExtensions
{
  /// <summary>Converts a <see cref="Direction"/> to a <see cref="Point2"/>.</summary>
  public static Point2 ToPoint2(this Direction direction)
  {
    var result = Point2.Zero;

    if (direction.HasFlagFast(Direction.North)) result.Y += 1;
    if (direction.HasFlagFast(Direction.South)) result.Y -= 1;
    if (direction.HasFlagFast(Direction.East)) result.X  -= 1;
    if (direction.HasFlagFast(Direction.West)) result.X  += 1;

    return result;
  }

  /// <summary>Converts a <see cref="Direction"/> to a <see cref="Vector2"/>.</summary>
  public static Vector2 ToVector2(this Direction direction)
  {
    var result = Vector2.Zero;

    if (direction.HasFlagFast(Direction.North)) result.Y += 1;
    if (direction.HasFlagFast(Direction.South)) result.Y -= 1;
    if (direction.HasFlagFast(Direction.East)) result.X  -= 1;
    if (direction.HasFlagFast(Direction.West)) result.X  += 1;

    return result;
  }
}

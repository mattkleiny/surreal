using Surreal.Collections;

namespace Surreal.Mathematics.Linear;

/// <summary>Possible cardinal directions.</summary>
[Flags]
public enum Direction : byte
{
  None  = 0,
  North = 1 << 1,
  East  = 1 << 2,
  South = 1 << 3,
  West  = 1 << 4,
  All   = North | East | South | West
}

public static class DirectionExtensions
{
  public static Direction NextDirection(this Random random)
  {
    return random.Next(4) switch
    {
      0 => Direction.North,
      1 => Direction.South,
      2 => Direction.East,
      3 => Direction.West,
      _ => throw new Exception("Unexpected value encountered!")
    };
  }

  public static Direction Opposite(this Direction direction)
  {
    var opposite = Direction.None;

    if (direction.HasFlagFast(Direction.North)) opposite |= Direction.South;
    if (direction.HasFlagFast(Direction.South)) opposite |= Direction.North;
    if (direction.HasFlagFast(Direction.East)) opposite  |= Direction.West;
    if (direction.HasFlagFast(Direction.West)) opposite  |= Direction.East;

    return opposite;
  }

  public static Point2 ToPoint(this Direction direction)
  {
    var point = Point2.Zero;

    if (direction.HasFlagFast(Direction.North)) point.Y += 1;
    if (direction.HasFlagFast(Direction.South)) point.Y -= 1;
    if (direction.HasFlagFast(Direction.East)) point.X  += 1;
    if (direction.HasFlagFast(Direction.West)) point.X  -= 1;

    return point;
  }
}
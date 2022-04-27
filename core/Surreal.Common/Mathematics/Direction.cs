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

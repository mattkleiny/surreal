using System;

namespace Surreal.Mathematics {
  [Flags]
  public enum Directions : byte {
    None  = 0,
    North = 1 << 1,
    East  = 1 << 2,
    South = 1 << 3,
    West  = 1 << 4,
    All   = North | East | South | West
  }
}
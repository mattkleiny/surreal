using System;

namespace Surreal.Framework.Tiles.Importers {
  [Flags]
  public enum FlipFlags : uint {
    None         = 0,
    Hexagonal120 = 0x10000000,
    Diagonal     = 0x20000000,
    Vertical     = 0x40000000,
    Horizontal   = 0x80000000,
    All          = Diagonal | Vertical | Horizontal | Hexagonal120,
  }
}
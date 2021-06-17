using Surreal.Mathematics.Linear;

namespace Surreal.Mathematics {
  public static class Neighbourhoods {
    public static readonly Point3[] Cardinal = {
      new(0, 1, 0),
      new(1, 0, 0),
      new(0, -1, 0),
      new(-1, 0, 0)
    };

    public static readonly Point3[] Diagonal = {
      new(-1, 1, 0),
      new(1, 1, 0),
      new(-1, -1, 0),
      new(1, -1, 0),
    };

    public static readonly Point3[] CardinalAndDiagonal = {
      new(-1, 1, 0),
      new(0, 1, 0),
      new(1, 1, 0),
      new(-1, 0, 0),
      new(1, 0, 0),
      new(-1, -1, 0),
      new(0, -1, 0),
      new(1, -1, 0),
    };
  }
}
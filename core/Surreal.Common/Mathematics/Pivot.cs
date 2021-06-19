using System;

namespace Surreal.Mathematics
{
  public readonly struct Pivot : IEquatable<Pivot>
  {
    public static readonly Pivot Min    = new(0f, 0f);
    public static readonly Pivot Max    = new(1f, 1f);
    public static readonly Pivot Center = new(0.5f, 0.5f);

    public Normal X { get; }
    public Normal Y { get; }

    public Pivot(Normal x, Normal y)
    {
      X = x;
      Y = y;
    }

    public override string ToString() => $"Pivot around <{X.ToString()} {Y.ToString()}>";

    public          bool Equals(Pivot other) => X.Equals(other.X) && Y.Equals(other.Y);
    public override bool Equals(object? obj) => obj is Pivot other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);
  }
}
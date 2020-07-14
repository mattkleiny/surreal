using System;

namespace Surreal.Mathematics.Linear {
  public readonly struct Area : IEquatable<Area> {
    public readonly float Width;
    public readonly float Height;

    public float Total => Width * Height;

    public Area(float width, float height) {
      Width  = width;
      Height = height;
    }

    public void Deconstruct(out float width, out float height) {
      width  = Width;
      height = Height;
    }

    public override string ToString() => $"{Width}x{Height} ({Total} units)";

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is Area other && Equals(other);
    }

    public bool Equals(Area other) {
      return Math.Abs(Width  - other.Width)  < float.Epsilon &&
             Math.Abs(Height - other.Height) < float.Epsilon;
    }

    public override int GetHashCode() => HashCode.Combine(Width, Height);

    public static bool operator ==(Area left, Area right) => left.Equals(right);
    public static bool operator !=(Area left, Area right) => !left.Equals(right);
  }
}
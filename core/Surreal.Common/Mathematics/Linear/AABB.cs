using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear {
  public readonly struct AABB : IEquatable<AABB> {
    public readonly Vector3 Min;
    public readonly Vector3 Max;

    public AABB(Vector3 min, Vector3 max) {
      Min = min;
      Max = max;
    }

    public void Deconstruct(out Vector3 min, out Vector3 max) {
      min = Min;
      max = Max;
    }

    public float X => Min.X;
    public float Y => Min.Y;
    public float Z => Min.Y;

    public float Width  => Max.X - Min.X;
    public float Height => Max.Y - Min.Y;
    public float Depth  => Max.Z - Min.Z;

    public bool Contains(Point3 point) {
      return point.X >= Min.X &&
             point.X <= Max.X &&
             point.Y >= Min.Y &&
             point.Y <= Max.Y &&
             point.Z >= Min.Z &&
             point.Z <= Max.Z;
    }

    public bool Contains(Vector3 vector) {
      return vector.X >= Min.X &&
             vector.X <= Max.X &&
             vector.Y >= Min.Y &&
             vector.Y <= Max.Y &&
             vector.Z >= Min.Z &&
             vector.Z <= Max.Z;
    }

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is AABB other && Equals(other);
    }

    public          bool Equals(AABB other) => Min.Equals(other.Min) && Max.Equals(other.Max);
    public override int  GetHashCode()      => HashCode.Combine(Min, Max);

    public static bool operator ==(AABB left, AABB right) => left.Equals(right);
    public static bool operator !=(AABB left, AABB right) => !left.Equals(right);
  }
}
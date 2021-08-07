using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear
{
  public readonly struct Frustum : IEquatable<Frustum>
  {
    public readonly Plane Left;
    public readonly Plane Right;
    public readonly Plane Top;
    public readonly Plane Bottom;
    public readonly Plane Near;
    public readonly Plane Far;

    public Frustum(
        Plane left,
        Plane right,
        Plane top,
        Plane bottom,
        Plane near,
        Plane far)
    {
      Left   = left;
      Right  = right;
      Top    = top;
      Bottom = bottom;
      Near   = near;
      Far    = far;
    }

    public void Deconstruct(
        out Plane left,
        out Plane right,
        out Plane top,
        out Plane bottom,
        out Plane near,
        out Plane far)
    {
      left   = Left;
      right  = Right;
      top    = Top;
      bottom = Bottom;
      near   = Near;
      far    = Far;
    }

    public bool Contains(Vector3 point)
    {
      if (Left.TestPoint(point) == HalfSpace.Behind) return false;
      if (Right.TestPoint(point) == HalfSpace.Behind) return false;
      if (Top.TestPoint(point) == HalfSpace.Behind) return false;
      if (Bottom.TestPoint(point) == HalfSpace.Behind) return false;
      if (Near.TestPoint(point) == HalfSpace.Behind) return false;
      if (Far.TestPoint(point) == HalfSpace.Behind) return false;

      return true;
    }

    public ContainmentType Contains(BoundingBox boundingBox)
    {
      throw new NotImplementedException();
    }

    public bool Equals(Frustum other)
    {
      return Left.Equals(other.Left) &&
             Right.Equals(other.Right) &&
             Top.Equals(other.Top) &&
             Bottom.Equals(other.Bottom) &&
             Near.Equals(other.Near) &&
             Far.Equals(other.Far);
    }

    public override bool Equals(object? obj) => obj is Frustum other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Left, Right, Top, Bottom, Near, Far);

    public static bool operator ==(Frustum left, Frustum right) => left.Equals(right);
    public static bool operator !=(Frustum left, Frustum right) => !left.Equals(right);

    public enum ContainmentType
    {
      Inside,
      Disjoint,
      Outside,
    }
  }
}
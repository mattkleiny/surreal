using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear {
  public readonly struct Frustum {
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
        Plane far) {
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
        out Plane far) {
      left   = Left;
      right  = Right;
      top    = Top;
      bottom = Bottom;
      near   = Near;
      far    = Far;
    }

    public bool Contains(Vector3 point) {
      if (Left.Test(point) == HalfSpace.Behind) return false;
      if (Right.Test(point) == HalfSpace.Behind) return false;
      if (Top.Test(point) == HalfSpace.Behind) return false;
      if (Bottom.Test(point) == HalfSpace.Behind) return false;
      if (Near.Test(point) == HalfSpace.Behind) return false;
      if (Far.Test(point) == HalfSpace.Behind) return false;

      return true;
    }

    public ContainmentType Contains(AABB aabb) {
      throw new NotImplementedException();
    }

    public enum ContainmentType {
      Inside,
      Disjoint,
      Outside,
    }
  }
}
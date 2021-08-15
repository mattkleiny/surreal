using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear
{
  public readonly record struct Frustum(Plane Left, Plane Right, Plane Top, Plane Bottom, Plane Near, Plane Far)
  {
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

    public enum ContainmentType
    {
      Inside,
      Disjoint,
      Outside,
    }
  }
}

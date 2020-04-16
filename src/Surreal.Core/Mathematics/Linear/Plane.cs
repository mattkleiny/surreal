using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear
{
  public static class PlaneExtensions
  {
    public static Halfspace Test(this Plane plane, Vector3 vector)
    {
      var dist = Plane.DotNormal(plane, vector) + plane.D;

      if (Math.Abs(dist) < float.Epsilon)
      {
        return Halfspace.OnPlane;
      }

      if (dist < 0)
      {
        return Halfspace.Behind;
      }

      return Halfspace.Front;
    }
  }
}

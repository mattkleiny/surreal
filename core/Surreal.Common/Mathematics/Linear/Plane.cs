using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear {
  public enum HalfSpace {
    Behind,
    OnPlane,
    Front,
  }

  public static class PlaneExtensions {
    public static HalfSpace Test(this Plane plane, Vector3 vector) {
      var dist = Plane.DotNormal(plane, vector) + plane.D;

      if (Math.Abs(dist) < float.Epsilon) {
        return HalfSpace.OnPlane;
      }

      if (dist < 0) {
        return HalfSpace.Behind;
      }

      return HalfSpace.Front;
    }
  }
}
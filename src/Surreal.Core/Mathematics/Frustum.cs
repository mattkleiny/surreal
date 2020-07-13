using System.Numerics;
using Surreal.Mathematics.Linear;

namespace Surreal.Mathematics {
  public readonly struct Frustum {
    public readonly Plane Left;
    public readonly Plane Right;
    public readonly Plane Top;
    public readonly Plane Bottom;
    public readonly Plane Near;
    public readonly Plane Far;

    public static Frustum Calculate(in Matrix4x4 projectionView) => new Frustum(
        left: new Plane(
            x: projectionView.M41 + projectionView.M11,
            y: projectionView.M42 + projectionView.M12,
            z: projectionView.M43 + projectionView.M13,
            d: projectionView.M44 + projectionView.M14
        ),
        right: new Plane(
            x: projectionView.M41 - projectionView.M11,
            y: projectionView.M42 - projectionView.M12,
            z: projectionView.M43 - projectionView.M13,
            d: projectionView.M44 - projectionView.M14
        ),
        top: new Plane(
            x: projectionView.M41 - projectionView.M21,
            y: projectionView.M42 - projectionView.M22,
            z: projectionView.M43 - projectionView.M23,
            d: projectionView.M44 - projectionView.M24
        ),
        bottom: new Plane(
            x: projectionView.M41 + projectionView.M21,
            y: projectionView.M42 + projectionView.M22,
            z: projectionView.M43 + projectionView.M23,
            d: projectionView.M44 + projectionView.M24
        ),
        near: new Plane(
            x: projectionView.M41 + projectionView.M31,
            y: projectionView.M42 + projectionView.M32,
            z: projectionView.M43 + projectionView.M33,
            d: projectionView.M44 + projectionView.M34
        ),
        far: new Plane(
            x: projectionView.M41 - projectionView.M31,
            y: projectionView.M42 - projectionView.M32,
            z: projectionView.M43 - projectionView.M33,
            d: projectionView.M44 - projectionView.M34
        )
    );

    public Frustum(Plane left, Plane right, Plane top, Plane bottom, Plane near, Plane far) {
      Left   = left;
      Right  = right;
      Top    = top;
      Bottom = bottom;
      Near   = near;
      Far    = far;
    }

    public bool Contains(Vector3 point) {
      if (Left.Test(point)   == Halfspace.Behind) return false;
      if (Right.Test(point)  == Halfspace.Behind) return false;
      if (Top.Test(point)    == Halfspace.Behind) return false;
      if (Bottom.Test(point) == Halfspace.Behind) return false;
      if (Near.Test(point)   == Halfspace.Behind) return false;
      if (Far.Test(point)    == Halfspace.Behind) return false;

      return true;
    }

    public ContainmentType Contains(AABB aabb) {
      // TODO: actually implement me

      if (Contains(aabb.Min) && Contains(aabb.Max)) {
        return ContainmentType.Inside;
      }

      return ContainmentType.Outside;
    }

    public enum ContainmentType {
      Inside,
      Disjoint,
      Outside
    }
  }
}
using System.Numerics;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Cameras {
  public interface ICamera {
    ref readonly Matrix4x4 ProjectionView { get; }
    ref readonly Frustum   Frustum        { get; }

    void Update();

    Point2 Project(Vector3 worldPosition);
    Vector3  Unproject(Point2 screenPosition);
  }
}
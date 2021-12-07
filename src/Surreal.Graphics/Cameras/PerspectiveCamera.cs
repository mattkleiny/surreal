using System.Numerics;
using static Surreal.Mathematics.Maths;

namespace Surreal.Graphics.Cameras;

/// <summary>A perspective <see cref="Camera" />.</summary>
public sealed class PerspectiveCamera : Camera
{
  private float fieldOfView = DegreesToRadians(75f);

  public PerspectiveCamera(int viewportWidth, int viewportHeight)
    : base(viewportWidth, viewportHeight)
  {
  }

  public float FieldOfView
  {
    get => RadiansToDegrees(fieldOfView);
    set
    {
      var radians = DegreesToRadians(value);
      fieldOfView = Clamp(radians, 0f, Tau);
    }
  }

  protected override void Recalculate(out Matrix4x4 projection)
  {
    projection = Matrix4x4.CreatePerspectiveFieldOfView(
      fieldOfView,
      (float) Viewport.Width / Viewport.Height,
      Near,
      Far
    );
  }
}
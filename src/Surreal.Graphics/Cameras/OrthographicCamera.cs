using System.Numerics;
using Surreal.Mathematics;

namespace Surreal.Graphics.Cameras
{
  public sealed class OrthographicCamera : Camera
  {
    private float zoom = 1f;

    public OrthographicCamera(int viewportWidth, int viewportHeight)
        : base(viewportWidth, viewportHeight)
    {
      Near      = 0f;
      Position  = new Vector3(0f, 0f, 0f);
      Direction = new Vector3(0f, 0f, -1f);
    }

    public float Zoom
    {
      get => zoom;
      set => zoom = Maths.Clamp(value, 0.1f, 10f);
    }

    protected override void Recalculate(out Matrix4x4 projection)
    {
      projection = Matrix4x4.CreateOrthographic(
          width: Zoom * Viewport.Width,
          height: Zoom * Viewport.Height,
          zNearPlane: Near,
          zFarPlane: Far
      );
    }
  }
}

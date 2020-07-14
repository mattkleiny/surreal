using System;
using System.Diagnostics;
using System.Numerics;
using Surreal.Mathematics;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Cameras {
  public abstract class Camera : ICamera {
    private float     near                  = 1f;
    private float     far                   = 100.0f;
    private Matrix4x4 view                  = Matrix4x4.Identity;
    private Matrix4x4 projection            = Matrix4x4.Identity;
    private Matrix4x4 projectionView        = Matrix4x4.Identity;
    private Matrix4x4 inverseProjectionView = Matrix4x4.Identity;
    private Frustum   frustum;

    protected Camera(int viewportWidth, int viewportHeight) {
      Debug.Assert(viewportWidth  > 0, "viewportWidth > 0");
      Debug.Assert(viewportHeight > 0, "viewportHeight > 0");

      Viewport = new Viewport(
          width: viewportWidth,
          height: viewportHeight
      );

      Update();
    }

    public event Action Updated = null!;

    public float Near {
      get => near;
      set => near = Maths.Clamp(value, 0f, float.MaxValue);
    }

    public float Far {
      get => far;
      set => far = Maths.Clamp(value, 0f, float.MaxValue);
    }

    public Vector3 Position  { get; set; } = Vector3.Zero;
    public Vector3 Direction { get; set; } = -Vector3.UnitZ;
    public Vector3 Up        { get; set; } = Vector3.UnitY;

    public Vector3 Forward => Vector3.Normalize(Direction - Position);
    public Vector3 Right   => Vector3.Normalize(Vector3.Cross(Forward, Up));

    public Viewport Viewport { get; set; }

    public ref readonly Matrix4x4 View                  => ref view;
    public ref readonly Matrix4x4 Projection            => ref projection;
    public ref readonly Matrix4x4 ProjectionView        => ref projectionView;
    public ref readonly Matrix4x4 InverseProjectionView => ref inverseProjectionView;
    public ref readonly Frustum   Frustum               => ref frustum;

    public void Translate(Vector3 amount) {
      Position  += amount;
      Direction += amount;
    }

    public void Update() {
      Recalculate(out projection);

      view = Matrix4x4.CreateLookAt(
          cameraPosition: Position,
          cameraTarget: Direction,
          cameraUpVector: Up
      );

      projectionView = view * projection;

      Matrix4x4.Invert(projectionView, out inverseProjectionView);
      frustum = Frustum.Calculate(in inverseProjectionView);

      Updated?.Invoke();
    }

    public Vector2I Project(Vector3 worldPosition) {
      var result = Vector3.Transform(worldPosition, projectionView);

      result.X = Viewport.Width  * (result.X + 1) / 2 + Viewport.X;
      result.Y = Viewport.Height * (result.Y + 1) / 2 + Viewport.Y;
      result.Z = (result.Z + 1) / 2f;

      return new Vector2I((int) result.X, (int) result.Y);
    }

    public Vector3 Unproject(Vector2I screenPosition) {
      var result = new Vector3(
          x: 2 * (screenPosition.X                       - Viewport.X) / Viewport.Width  - 1,
          y: 2 * (Viewport.Height - screenPosition.Y - 1 - Viewport.Y) / Viewport.Height - 1,
          z: -1
      );

      return Vector3.Transform(result, inverseProjectionView);
    }

    protected abstract void Recalculate(out Matrix4x4 projection);
  }
}
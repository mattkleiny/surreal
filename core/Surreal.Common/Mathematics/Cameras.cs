namespace Surreal.Mathematics;

/// <summary>A camera which allows computation of view-projection matrices from source information.</summary>
public sealed class Camera
{
  private Vector2 position = Vector2.Zero;
  private Vector2 size = new(1920, 1080);
  private float nearPlane = 0f;
  private float farPlane = 100f;

  private Matrix4x4 view = Matrix4x4.Identity;
  private Matrix4x4 projection = Matrix4x4.Identity;
  private Matrix4x4 projectionView = Matrix4x4.Identity;

  public Vector2 Position
  {
    get => position;
    set
    {
      position = value;
      RecomputeViewProjection();
    }
  }

  public Vector2 Size
  {
    get => size;
    set
    {
      size = value;
      RecomputeViewProjection();
    }
  }

  public float NearPlane
  {
    get => nearPlane;
    set
    {
      nearPlane = value;
      RecomputeViewProjection();
    }
  }

  public float FarPlane
  {
    get => farPlane;
    set
    {
      farPlane = value;
      RecomputeViewProjection();
    }
  }

  public ref readonly Matrix4x4 View           => ref view;
  public ref readonly Matrix4x4 Projection     => ref projection;
  public ref readonly Matrix4x4 ProjectionView => ref projectionView;

  private void RecomputeViewProjection()
  {
    view       = Matrix4x4.CreateTranslation(position.X, position.Y, 0f);
    projection = Matrix4x4.CreateOrthographic(size.X, size.Y, nearPlane, farPlane);

    projectionView = view * projection;
  }
}

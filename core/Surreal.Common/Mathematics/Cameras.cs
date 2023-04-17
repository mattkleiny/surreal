namespace Surreal.Mathematics;

/// <summary>
/// A camera which allows computation of view-projection matrices from source information.
/// </summary>
public sealed class Camera
{
  private float _farPlane = 100f;
  private float _nearPlane;
  private Vector2 _position = Vector2.Zero;
  private Matrix4x4 _projection = Matrix4x4.Identity;
  private Matrix4x4 _projectionView = Matrix4x4.Identity;
  private Vector2 _size = new(1920, 1080);

  private Matrix4x4 _view = Matrix4x4.Identity;

  public Vector2 Position
  {
    get => _position;
    set
    {
      _position = value;
      RecomputeViewProjection();
    }
  }

  public Vector2 Size
  {
    get => _size;
    set
    {
      _size = value;
      RecomputeViewProjection();
    }
  }

  public float NearPlane
  {
    get => _nearPlane;
    set
    {
      _nearPlane = value;
      RecomputeViewProjection();
    }
  }

  public float FarPlane
  {
    get => _farPlane;
    set
    {
      _farPlane = value;
      RecomputeViewProjection();
    }
  }

  public ref readonly Matrix4x4 View => ref _view;
  public ref readonly Matrix4x4 Projection => ref _projection;
  public ref readonly Matrix4x4 ProjectionView => ref _projectionView;

  private void RecomputeViewProjection()
  {
    _view = Matrix4x4.CreateTranslation(_position.X, _position.Y, 0f);
    _projection = Matrix4x4.CreateOrthographic(_size.X, _size.Y, _nearPlane, _farPlane);

    _projectionView = _view * _projection;
  }
}

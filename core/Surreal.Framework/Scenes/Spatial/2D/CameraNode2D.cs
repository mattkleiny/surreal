using Surreal.Colors;
using Surreal.Maths;
using Surreal.Timing;

namespace Surreal.Scenes.Spatial;

/// <summary>
/// A <see cref="SceneNode2D"/> that represents a camera.
/// </summary>
public class CameraNode2D : SceneNode2D, ICamera
{
  private Matrix4x4 _projectionView = Matrix4x4.Identity;
  private Frustum _frustum = Frustum.FromProjectionMatrix(Matrix4x4.Identity);
  private float _zoom = 4.5f;
  private float _aspectRatio = 1;
  private float _nearPlane;
  private float _farPlane = 100f;
  private bool _isCameraDirty;

  /// <inheritdoc/>
  public ref readonly Frustum Frustum => ref _frustum;

  /// <inheritdoc/>
  public ref readonly Matrix4x4 ProjectionView => ref _projectionView;

  /// <inheritdoc/>
  public Optional<Color> ClearColor { get; set; }

  /// <summary>
  /// The zoom level of the camera.
  /// </summary>
  public float Zoom
  {
    get => _zoom;
    set
    {
      if (SetField(ref _zoom, value.Clamp(1f, 1000f)))
      {
        _isCameraDirty = true;
      }
    }
  }

  /// <summary>
  /// The aspect ratio of the camera.
  /// </summary>
  public float AspectRatio
  {
    get => _aspectRatio;
    set
    {
      if (SetField(ref _aspectRatio, value.Clamp(0f, 100f)))
      {
        _isCameraDirty = true;
      }
    }
  }

  /// <summary>
  /// The distance to the near plane.
  /// </summary>
  public float NearPlane
  {
    get => _nearPlane;
    set
    {
      if (SetField(ref _nearPlane, value.Clamp(0f, float.MaxValue)))
      {
        _isCameraDirty = true;
      }
    }
  }

  /// <summary>
  /// The distance to the far plane.
  /// </summary>
  public float FarPlane
  {
    get => _farPlane;
    set
    {
      if (SetField(ref _farPlane, value.Clamp(0f, float.MaxValue)))
      {
        _isCameraDirty = true;
      }
    }
  }

  protected override void OnEnterTree()
  {
    base.OnEnterTree();

    if (TryResolveParent(out CameraViewportNode viewport))
    {
      viewport.ActiveCameras.AddFirst(this);
    }
  }

  protected override void OnExitTree()
  {
    if (TryResolveParent(out CameraViewportNode viewport))
    {
      viewport.ActiveCameras.Remove(this);
    }

    base.OnExitTree();
  }

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    if (_isCameraDirty)
    {
      OnTransformUpdated();

      _isCameraDirty = false;
    }
  }

  protected override void OnTransformUpdated()
  {
    base.OnTransformUpdated();

    // create projection/view matrix from global position and orthographic projection
    _projectionView = Matrix4x4.CreateOrthographicOffCenter(
      left: GlobalPosition.X - _aspectRatio * _zoom,
      right: GlobalPosition.X + _aspectRatio * _zoom,
      bottom: GlobalPosition.Y - _zoom,
      top: GlobalPosition.Y + _zoom,
      zNearPlane: _nearPlane,
      zFarPlane: _farPlane
    );

    // create frustum from projection matrix
    _frustum = Frustum.FromProjectionMatrix(_projectionView);
  }
}

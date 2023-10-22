using Surreal.Collections;
using Surreal.Graphics.Rendering;
using Surreal.Maths;

namespace Surreal.Scenes.Canvas;

/// <summary>
/// A <see cref="SceneNode2D"/> that represents a camera.
/// </summary>
public class CameraNode2D : SceneNode2D, IRenderCamera
{
  private Matrix4x4 _projectionView = Matrix4x4.Identity;
  private Frustum _frustum = Frustum.FromProjectionMatrix(Matrix4x4.Identity);
  private float _zoom = 4.5f;
  private float _aspectRatio = 1;
  private float _nearPlane;
  private float _farPlane = 100f;

  /// <summary>
  /// The zoom level of the camera.
  /// </summary>
  public float Zoom
  {
    get => _zoom;
    set => SetField(ref _zoom, value.Clamp(0f, 100f));
  }

  /// <summary>
  /// The aspect ratio of the camera.
  /// </summary>
  public float AspectRatio
  {
    get => _aspectRatio;
    set => SetField(ref _aspectRatio, value.Clamp(0f, 100f));
  }

  /// <summary>
  /// The distance to the near plane.
  /// </summary>
  public float NearPlane
  {
    get => _nearPlane;
    set => SetField(ref _nearPlane, value.Clamp(0f, float.MaxValue));
  }

  /// <summary>
  /// The distance to the far plane.
  /// </summary>
  public float FarPlane
  {
    get => _farPlane;
    set => SetField(ref _farPlane, value.Clamp(0f, float.MaxValue));
  }

  /// <summary>
  /// The frustum of the camera.
  /// </summary>
  public ref readonly Frustum Frustum => ref _frustum;

  /// <inheritdoc/>
  public ref readonly Matrix4x4 ProjectionView => ref _projectionView;

  /// <inheritdoc/>
  ReadOnlySlice<IRenderObject> IRenderCamera.CullVisibleObjects()
  {
    if (TryResolveRoot(out SceneNode node))
    {
      return node.ResolveChildren<IRenderObject>(_ => _.IsVisibleToFrustum(_frustum));
    }

    return ReadOnlySlice<IRenderObject>.Empty;
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

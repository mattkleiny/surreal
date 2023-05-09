namespace Surreal.Graphics.Rendering;

/// <summary>
/// Represents a kind of camera that can be used to render a scene.
/// </summary>
public interface IRenderCamera
{
  /// <summary>
  /// The projection-view matrix for the camera.
  /// </summary>
  ref readonly Matrix4x4 ProjectionView { get; }
}

using Surreal.Collections;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Represents an object that has been culled by a camera.
/// </summary>
public readonly struct CulledObject;

/// <summary>
/// Represents a kind of camera that can be used to render a scene.
/// </summary>
public interface IRenderCamera
{
  /// <summary>
  /// The projection-view matrix for the camera.
  /// </summary>
  ref readonly Matrix4x4 ProjectionView { get; }

  /// <summary>
  /// Culls visible objects from the perspective of the camera.
  /// </summary>
  ReadOnlySlice<CulledObject> CullVisibleObjects();
}

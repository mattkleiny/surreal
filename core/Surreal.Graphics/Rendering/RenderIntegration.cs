using Surreal.Collections;
using Surreal.Maths;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Represents a scene that can be rendered by a <see cref="IRenderPipeline"/>.
/// </summary>
public interface IRenderScene
{
  /// <summary>
  /// Culls visible cameras from the scene.
  /// </summary>
  ReadOnlySlice<IRenderCamera> CullVisibleCameras();
}

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
  ReadOnlySlice<IRenderObject> CullVisibleObjects();
}

/// <summary>
/// Represents a kind of object that can be rendered to the screen.
/// </summary>
public interface IRenderObject
{
  /// <summary>
  /// Determines if the object is visible to the given frustum.
  /// </summary>
  bool IsVisibleToFrustum(in Frustum frustum);

  /// <summary>
  /// Renders the object.
  /// </summary>
  void Render(in RenderFrame frame);
}

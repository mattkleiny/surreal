using Surreal.Collections;
using Surreal.Maths;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Represents a scene that can be rendered by a <see cref="IRenderPipeline"/>.
/// </summary>
public interface IRenderScene
{
  /// <summary>
  /// Culls active <see cref="IRenderViewport"/>s in the scene.
  /// </summary>
  ReadOnlySlice<IRenderViewport> CullActiveViewports();
}

/// <summary>
/// Represents a kind of viewport that can be used to render a scene.
/// </summary>
public interface IRenderViewport
{
  /// <summary>
  /// The projection-view matrix for the camera.
  /// </summary>
  ref readonly Matrix4x4 ProjectionView { get; }

  /// <summary>
  /// Culls visible objects from the perspective of the camera.
  /// </summary>
  ReadOnlySlice<T> CullVisibleObjects<T>()
    where T : class;
}

/// <summary>
/// Determines if an object is visible to the camera.
/// </summary>
public interface ICullableObject
{
  /// <summary>
  /// Determines if the object is visible to the given frustum.
  /// </summary>
  bool IsVisibleToFrustum(in Frustum frustum);
}

/// <summary>
/// Represents a kind of object that can be rendered to the screen.
/// </summary>
public interface IRenderObject
{
  /// <summary>
  /// Renders the object.
  /// </summary>
  void Render(in RenderFrame frame);
}

using Surreal.Collections.Slices;
using Surreal.Mathematics;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Represents a scene that can be rendered by a <see cref="IRenderPipeline"/>.
/// </summary>
public interface IRenderScene
{
  static IRenderScene Null { get; } = new NullRenderScene();

  /// <summary>
  /// Culls active <see cref="IRenderViewport"/>s in the scene.
  /// </summary>
  ReadOnlySlice<IRenderViewport> CullActiveViewports();

  /// <summary>
  /// A no-op <see cref="IRenderScene"/>.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullRenderScene : IRenderScene
  {
    private static readonly IRenderViewport[] Viewports = [IRenderViewport.Null];

    public ReadOnlySlice<IRenderViewport> CullActiveViewports()
    {
      return Viewports;
    }
  }
}

/// <summary>
/// Represents a kind of viewport that can be used to render a scene.
/// </summary>
public interface IRenderViewport
{
  static IRenderViewport Null { get; } = new NullRenderViewport();

  /// <summary>
  /// The projection-view matrix for the camera.
  /// </summary>
  ref readonly Matrix4x4 ProjectionView { get; }

  /// <summary>
  /// Culls visible objects from the perspective of the camera.
  /// </summary>
  ReadOnlySlice<T> CullVisibleObjects<T>()
    where T : class;

  /// <summary>
  /// A no-op <see cref="IRenderViewport"/>.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullRenderViewport : IRenderViewport
  {
    private readonly Matrix4x4 _projectionView = Matrix4x4.Identity;

    public ref readonly Matrix4x4 ProjectionView => ref _projectionView;

    public ReadOnlySlice<T> CullVisibleObjects<T>()
      where T : class
    {
      return ReadOnlySlice<T>.Empty;
    }
  }
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

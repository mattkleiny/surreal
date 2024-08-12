using Surreal.Collections.Slices;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Sprites;

namespace Surreal.Scenes;

/// <summary>
/// A node that renders a GUI in canvas space.
/// </summary>
public sealed class CanvasViewport : SceneNode, IRenderViewport, IRenderObject
{
  private readonly Matrix4x4 _projectionView = Matrix4x4.Identity;

  public ref readonly Matrix4x4 ProjectionView => ref _projectionView;

  ReadOnlySlice<T> IRenderViewport.CullVisibleObjects<T>()
  {
    if (this is T instance)
    {
      return new[] { instance };
    }

    return ReadOnlySlice<T>.Empty;
  }

  void IRenderObject.Render(in RenderFrame frame)
  {
    if (frame.Contexts.TryGetContext(in frame, out CanvasContext context))
    {
    }
  }
}

using Surreal.Collections;
using Surreal.Graphics.Rendering;
using Surreal.Maths;

namespace Surreal.Scenes.UI;

/// <summary>
/// A node that renders UI <see cref="UI.Widget"/>s to the screen.
/// </summary>
public sealed class WidgetViewportNode : SceneNode, IRenderViewport, IRenderObject
{
  private readonly Matrix4x4 _projectionView = Matrix4x4.CreateOrthographic(256, 144, 0f, 100f);

  /// <summary>
  /// The widget to render.
  /// </summary>
  public required Widget Widget { get; init; }

  /// <inheritdoc/>
  public ref readonly Matrix4x4 ProjectionView => ref _projectionView;

  /// <inheritdoc/>
  ReadOnlySlice<IRenderObject> IRenderViewport.CullVisibleObjects()
  {
    return ResolveChildren<IRenderObject>();
  }

  bool IRenderObject.IsVisibleToFrustum(in Frustum frustum)
  {
    return true;
  }

  void IRenderObject.Render(in RenderFrame frame)
  {
    if (frame.Contexts.TryGetContext(in frame, out WidgetContext context))
    {
      Widget.Render(in frame, context.WidgetBatch);
    }
  }
}

using Surreal.Collections;
using Surreal.Colors;
using Surreal.Graphics.Rendering;

namespace Surreal.Scenes.UI;

/// <summary>
/// A node that renders UI elements to the screen.
/// </summary>
public sealed class CanvasViewportNode : SceneNode, IRenderViewport
{
  /// <inheritdoc/>
  public Optional<Color> ClearColor { get; set; }

  /// <inheritdoc/>
  public ref readonly Matrix4x4 ProjectionView => throw new NotImplementedException();

  /// <inheritdoc/>
  ReadOnlySlice<IRenderObject> IRenderViewport.CullVisibleObjects()
  {
    throw new NotImplementedException();
  }
}

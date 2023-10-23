using Surreal.Graphics;
using Surreal.Graphics.Gizmos;
using Surreal.Graphics.Rendering;
using Surreal.Maths;

namespace Surreal.Scenes.Spatial;

public class TileMap;

/// <summary>
/// A node that renders a tile map.
/// </summary>
public class TileMapNode2D : SceneNode2D, ICullableObject, IRenderObject, IGizmoObject
{
  /// <summary>
  /// The tile map to render.
  /// </summary>
  public TileMap? TileMap { get; set; }

  bool ICullableObject.IsVisibleToFrustum(in Frustum frustum)
  {
    return true;
  }

  void IRenderObject.Render(in RenderFrame frame)
  {
    if (frame.Contexts.TryGetContext(in frame, out SpriteContext context))
    {
    }
  }

  void IGizmoObject.RenderGizmos(in RenderFrame frame, GizmoBatch gizmos)
  {
  }
}

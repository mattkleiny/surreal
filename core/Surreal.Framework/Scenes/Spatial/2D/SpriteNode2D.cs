using Surreal.Colors;
using Surreal.Graphics;
using Surreal.Graphics.Gizmos;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Maths;

namespace Surreal.Scenes.Spatial;

/// <summary>
/// A node that renders a sprite.
/// </summary>
public class SpriteNode2D : SceneNode2D, ICullableObject, IRenderObject, IGizmoObject
{
  /// <summary>
  /// The sprite texture to render.
  /// </summary>
  public TextureRegion Sprite { get; set; } = TextureRegion.Empty;

  /// <summary>
  /// The tint to apply to the sprite.
  /// </summary>
  public Color Tint { get; set; } = Color.White;

  protected virtual void OnRender(SpriteBatch batch)
  {
    batch.Draw(
      region: Sprite,
      position: GlobalPosition,
      size: GlobalScale * Sprite.Size,
      angle: GlobalRotation.Radians,
      color: Tint
    );
  }

  bool ICullableObject.IsVisibleToFrustum(in Frustum frustum)
  {
    var center = new Vector3(GlobalPosition.X, GlobalPosition.Y, 0f);
    var size = MathF.Max(Sprite.Size.X, Sprite.Size.Y) / 2f;

    return frustum.ContainsSphere(center, size);
  }

  void IRenderObject.Render(in RenderFrame frame)
  {
    if (frame.Contexts.TryGetContext(in frame, out SpriteContext context))
    {
      OnRender(context.SpriteBatch);
    }
  }

  void IGizmoObject.RenderGizmos(in RenderFrame frame, GizmoBatch gizmos)
  {
    gizmos.DrawWireQuad(
      center: GlobalPosition,
      size: GlobalScale * Sprite.Size,
      color: Color.Red
    );
  }
}

﻿using Surreal.Colors;
using Surreal.Graphics;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;

namespace Surreal.Scenes.Canvas;

/// <summary>
/// A node that renders a sprite.
/// </summary>
public class SpriteNode : SceneNode2D, IRenderObject
{
  /// <summary>
  /// The sprite texture to render.
  /// </summary>
  public TextureRegion Sprite { get; set; } = TextureRegion.Empty;

  /// <summary>
  /// The tint to apply to the sprite.
  /// </summary>
  public Color Tint { get; set; } = Color.White;

  void IRenderObject.Render(in RenderFrame frame)
  {
    if (frame.Manager.TryGetContext(in frame, out SpriteBatchContext context))
    {
      context.Batch.Draw(
        region: Sprite,
        position: GlobalPosition,
        size: GlobalScale,
        angle: GlobalRotation.Radians,
        color: Tint
      );
    }
  }
}

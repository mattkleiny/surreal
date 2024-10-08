﻿using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Scenes.Canvas;

/// <summary>
/// A node that renders a sprite.
/// </summary>
public class Sprite2D : Node2D, ICullableObject, IRenderObject
{
  private Material? _material;
  private TextureRegion _sprite = TextureRegion.Empty;
  private Color _tint = Color.White;

  /// <summary>
  /// The material to use when rendering the sprite.
  /// </summary>
  public Material? Material
  {
    get => _material;
    set => SetField(ref _material, value);
  }

  /// <summary>
  /// The sprite texture to render.
  /// </summary>
  public TextureRegion Sprite
  {
    get => _sprite;
    set => SetField(ref _sprite, value);
  }

  /// <summary>
  /// The tint to apply to the sprite.
  /// </summary>
  public Color Tint
  {
    get => _tint;
    set => SetField(ref _tint, value);
  }

  protected virtual void OnRender(in RenderFrame frame, SpriteBatch batch)
  {
    batch.DrawQuad(
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
      OnRender(in frame, context.Batch);
    }
  }
}

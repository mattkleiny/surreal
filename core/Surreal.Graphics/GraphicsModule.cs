using Surreal.Assets;
using Surreal.Entities;
using Surreal.Graphics.Images;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Graphics.Utilities;
using Surreal.Services;

namespace Surreal.Graphics;


/// <summary>
/// A <see cref="IServiceModule"/> for the graphics system.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class GraphicsModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService<IAssetLoader, AsepriteFileLoader>();
    registry.AddService<IAssetLoader, ColorPaletteLoader>();
    registry.AddService<IAssetLoader, ImageLoader>();
    registry.AddService<IAssetLoader, MaterialLoader>();
    registry.AddService<IAssetLoader, ShaderProgramLoader>();
    registry.AddService<IAssetLoader, TextureLoader>();

    registry.AddSystem<RenderFrame>(RenderSprites);
  }

  /// <summary>
  /// A system for rendering sprites.
  /// </summary>
  private void RenderSprites(in RenderFrame @event, Transform transform, Sprite sprite)
  {
    if (@event.TryGetContext(out SpriteContext context))
    {
      context.Batch.DrawQuad(
        region: sprite.Region,
        position: transform.Position,
        size: transform.Scale,
        angle: transform.Rotation,
        color: sprite.Tint
      );
    }
  }
}

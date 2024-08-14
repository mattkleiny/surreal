using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Sprites.Aseprite;
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
    registry.AddService<IAssetLoader, AsepriteSpriteAnimationLoader>();
    registry.AddService<IAssetLoader, AsepriteSpriteAnimationSetLoader>();
    registry.AddService<IAssetLoader, AsepriteTextureLoader>();
    registry.AddService<IAssetLoader, AsepriteTextureAtlasLoader>();
    registry.AddService<IAssetLoader, ColorPaletteLoader>();
    registry.AddService<IAssetLoader, ImageLoader>();
    registry.AddService<IAssetLoader, MaterialLoader>();
    registry.AddService<IAssetLoader, ShaderProgramLoader>();
    registry.AddService<IAssetLoader, TextureLoader>();
  }
}

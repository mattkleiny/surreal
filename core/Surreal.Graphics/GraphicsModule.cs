using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Textures;
using Surreal.Graphics.Utilities;
using Surreal.Utilities;

namespace Surreal.Graphics;

/// <summary>
/// A <see cref="IServiceModule"/> for the graphics system.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class GraphicsModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    var backend = registry.GetServiceOrThrow<IGraphicsBackend>();

    registry.AddService<IAssetLoader>(new ColorPaletteLoader());
    registry.AddService<IAssetLoader>(new ImageLoader());
    registry.AddService<IAssetLoader>(new MaterialLoader(backend));
    registry.AddService<IAssetLoader>(new ShaderProgramLoader(backend));
    registry.AddService<IAssetLoader>(new TextureLoader(backend));
  }
}

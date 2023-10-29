using Surreal.Assets;
using Surreal.Colors;
using Surreal.Graphics.Images;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Textures;
using Surreal.Graphics.Utilities;
using Surreal.Utilities;

namespace Surreal.Graphics;

/// <summary>
/// A <see cref="IServiceModule"/> for the <see cref="Graphics"/> namespace.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class GraphicsModule(IGraphicsBackend backend) : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService<IAssetLoader>(new ColorPaletteLoader());
    registry.AddService<IAssetLoader>(new ImageLoader());
    registry.AddService<IAssetLoader>(new MaterialLoader(backend));
    registry.AddService<IAssetLoader>(new ShaderProgramLoader(backend));
    registry.AddService<IAssetLoader>(new TextureLoader(backend));
  }
}

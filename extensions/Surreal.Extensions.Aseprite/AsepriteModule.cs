using Surreal.Assets;
using Surreal.Graphics;
using Surreal.Graphics.Sprites;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A <see cref="IServiceModule"/> for the Aseprite assembly.
/// </summary>
public sealed class AsepriteModule(IGraphicsBackend backend) : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService<IAssetLoader>(new AsepriteSpriteSheetLoader(backend));
  }
}

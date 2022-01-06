using NUnit.Framework;
using Surreal.Assets;
using Surreal.Graphics.Images;

namespace Surreal.Graphics.Textures;

public class TextureLoaderTests
{
  [Test, AutoFixture]
  public async Task it_should_load_a_texture(IGraphicsDevice device)
  {
    using var manager = new AssetManager();

    manager.AddLoader(new ImageLoader());
    manager.AddLoader(new TextureLoader(device, TextureFilterMode.Point, TextureWrapMode.Clamp, hotReloading: false));

    await manager.LoadAsset<Texture>("Assets/images/test.png");

    device.Received(1).CreateTexture(Arg.Any<ITextureData>(), TextureFilterMode.Point, TextureWrapMode.Clamp);
  }
}

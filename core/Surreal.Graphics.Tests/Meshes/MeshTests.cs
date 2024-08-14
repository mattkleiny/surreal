using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Meshes;

public class MeshTests
{
  [Test]
  public void it_should_create_a_texture()
  {
    var device = Substitute.For<IGraphicsDevice>();

    using var texture = new Texture(device, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);

    device.Received(1).CreateTexture(TextureFilterMode.Point, TextureWrapMode.ClampToEdge);
  }

  [Test]
  public void it_should_dispose_texture()
  {
    var device = Substitute.For<IGraphicsDevice>();
    var texture = new Texture(device, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);

    texture.Dispose();

    device.Received(1).DeleteTexture(texture.Handle);
  }
}

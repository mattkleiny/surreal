using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Meshes;

public class MeshTests
{
  [Test]
  public void it_should_create_a_texture()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var texture = new Texture(backend, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.Clamp);

    backend.Received(1).CreateTexture(TextureFilterMode.Point, TextureWrapMode.Clamp);
  }

  [Test]
  public void it_should_dispose_texture()
  {
    var backend = Substitute.For<IGraphicsBackend>();
    var texture = new Texture(backend, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.Clamp);

    texture.Dispose();

    backend.Received(1).DeleteTexture(texture.Handle);
  }
}

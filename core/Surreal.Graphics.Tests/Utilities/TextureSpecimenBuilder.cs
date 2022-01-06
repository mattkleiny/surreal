using AutoFixture.Kernel;
using Surreal.Graphics.Images;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Utilities;

[RegisterSpecimenBuilder]
internal sealed class TextureSpecimenBuilder : SpecimenBuilder<Texture>
{
  protected override Texture Create(ISpecimenContext context, string? name = null)
  {
    var texture = new InMemoryTexture(TextureFormat.Rgba8888, TextureFilterMode.Point, TextureWrapMode.Clamp);
    var data    = new Image(16, 16);

    texture.Upload(data);

    return texture;
  }
}

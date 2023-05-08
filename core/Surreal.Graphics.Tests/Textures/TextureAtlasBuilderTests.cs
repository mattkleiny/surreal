using Surreal.Colors;

namespace Surreal.Graphics.Textures;

public class TextureAtlasBuilderTests
{
  [Test]
  public async Task it_should_convert_to_image()
  {
    var builder = new TextureAtlasBuilder();

    builder.AddCell(16, 16).Span.Fill(ColorB.Red);
    builder.AddCell(16, 16).Span.Fill(ColorB.Green);
    builder.AddCell(16, 16).Span.Fill(ColorB.Blue);
    builder.AddCell(16, 16).Span.Fill(ColorB.Yellow);
    builder.AddCell(16, 16).Span.Fill(ColorB.Cyan);
    builder.AddCell(16, 16).Span.Fill(ColorB.Magenta);

    var result = builder.ToImage(3);

    await result.SaveAsync("output.png");
  }
}

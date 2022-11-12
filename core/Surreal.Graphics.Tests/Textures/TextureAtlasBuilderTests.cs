using Surreal.Mathematics;

namespace Surreal.Graphics.Textures;

public class TextureAtlasBuilderTests
{
  [Test]
  public async Task it_should_convert_to_image()
  {
    var builder = new TextureAtlasBuilder();

    builder.AddCell(16, 16).Span.Fill(Color32.Red);
    builder.AddCell(16, 16).Span.Fill(Color32.Green);
    builder.AddCell(16, 16).Span.Fill(Color32.Blue);
    builder.AddCell(16, 16).Span.Fill(Color32.Yellow);
    builder.AddCell(16, 16).Span.Fill(Color32.Cyan);
    builder.AddCell(16, 16).Span.Fill(Color32.Magenta);

    var result = builder.ToImage(3);

    await result.SaveAsync("output.png");
  }
}




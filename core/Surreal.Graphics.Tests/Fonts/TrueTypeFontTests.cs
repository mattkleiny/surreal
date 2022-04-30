using Surreal.Assets;

namespace Surreal.Graphics.Fonts;

public class TrueTypeFontTests
{
  [Test]
  public async Task it_should_load_a_true_type_font()
  {
    var server = new HeadlessGraphicsServer();

    using var manager = new AssetManager();

    manager.AddLoader(new TrueTypeFontLoader(server));

    var font = await manager.LoadAssetAsync<TrueTypeFont>("Assets/fonts/bitboy8_v1.ttf");

    font.Should().NotBeNull();
  }

  [Test]
  public async Task it_should_rasterize_fonts_to_requested_size()
  {
    var server = new HeadlessGraphicsServer();

    using var manager = new AssetManager();

    manager.AddLoader(new TrueTypeFontLoader(server));

    var font = await manager.LoadAssetAsync<TrueTypeFont>("Assets/fonts/bitboy8_v1.ttf");
    var rasterized = font.GetFont(16f);

    var size = rasterized.MeasureSize("Hello, World!");

    size.Width.Should().BeGreaterThan(0);
    size.Height.Should().BeGreaterThan(0);
  }
}

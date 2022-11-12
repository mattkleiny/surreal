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
}






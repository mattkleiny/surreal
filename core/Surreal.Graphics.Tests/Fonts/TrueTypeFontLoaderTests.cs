using Surreal.Assets;

namespace Surreal.Graphics.Fonts;

public class TrueTypeFontLoaderTests
{
  [Test]
  [TestCase("Assets/fonts/arial.ttf")]
  public async Task it_should_load_a_font(string path)
  {
    using var manager = new AssetManager();

    manager.AddLoader(new TrueTypeFontLoader());

    var font = await manager.LoadAsset<TrueTypeFont>(path);

    Assert.IsNotNull(font);
  }
}

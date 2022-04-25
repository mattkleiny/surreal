using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.Graphics.Meshes;

namespace Surreal.Graphics.Fonts;

public class BitmapFontTests
{
  [Test]
  public async Task it_should_load_a_bitmap_font()
  {
    var server = new HeadlessGraphicsServer();

    using var manager = new AssetManager();

    manager.AddLoader(new BitmapFontLoader(server));
    manager.AddLoader(new ImageLoader());

    var font = await manager.LoadAsset<BitmapFont>("Assets/fonts/IBM.font");

    font.Should().NotBeNull();
  }

  [Test]
  public async Task it_should_draw_to_a_sprite_batch()
  {
    var server = new HeadlessGraphicsServer();

    using var manager = new AssetManager();
    using var batch = new SpriteBatch(server, spriteCount: 128);

    manager.AddLoader(new BitmapFontLoader(server));
    manager.AddLoader(new ImageLoader());

    var font = await manager.LoadAsset<BitmapFont>("Assets/fonts/IBM.font");

    batch.DrawText(font, "This is a test", Vector2.Zero);
  }
}

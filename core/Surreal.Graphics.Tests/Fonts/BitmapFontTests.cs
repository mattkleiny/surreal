using Surreal.Colors;
using Surreal.Graphics.Images;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Resources;

namespace Surreal.Graphics.Fonts;

public class BitmapFontTests
{
  [Test]
  public async Task it_should_load_a_bitmap_font()
  {
    var server = new HeadlessGraphicsServer();

    using var manager = new ResourceManager();

    manager.AddLoader(new BitmapFontLoader());
    manager.AddLoader(new TextureLoader(server));
    manager.AddLoader(new ImageLoader());

    var font = await manager.LoadResourceAsync<BitmapFont>("Assets/fonts/IBM.font");

    font.Should().NotBeNull();
  }

  [Test]
  public async Task it_should_draw_to_a_sprite_batch()
  {
    var server = new HeadlessGraphicsServer();

    using var manager = new ResourceManager();
    using var batch = new SpriteBatch(server, 128);

    manager.AddLoader(new BitmapFontLoader());
    manager.AddLoader(new TextureLoader(server));
    manager.AddLoader(new ImageLoader());

    var font = await manager.LoadResourceAsync<BitmapFont>("Assets/fonts/IBM.font");

    batch.DrawText(font, "This is a test", Vector2.Zero, Vector2.One, ColorF.White);
  }
}

using Surreal.Assets;

namespace Surreal.Graphics.Images;

public class ImageLoaderTests
{
  [Test]
  [TestCase("Assets/images/test.png")]
  public async Task it_should_load_an_image(string path)
  {
    using var manager = new AssetManager();

    manager.AddLoader(new ImageLoader());

    var image = await manager.LoadAssetAsync<Image>(path);

    image.Width.Should().BeGreaterThan(0);
    image.Height.Should().BeGreaterThan(0);
    image.Size.Bytes.Should().BeGreaterThan(0);
  }
}




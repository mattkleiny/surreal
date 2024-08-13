using Surreal.Assets;

namespace Surreal.Graphics.Images;

public class ImageLoaderTests
{
  [Test]
  [TestCase("Assets/External/images/test.png")]
  public async Task it_should_load_an_image(string path)
  {
    using var manager = new AssetManager();

    manager.AddLoader(new ImageLoader());

    var image = await manager.LoadAsync<Image>(path);

    image.Value.Width.Should().BeGreaterThan(0);
    image.Value.Height.Should().BeGreaterThan(0);
    image.Value.Size.Bytes.Should().BeGreaterThan(0);
  }
}

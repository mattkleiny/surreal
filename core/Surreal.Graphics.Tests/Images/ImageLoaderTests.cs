using Surreal.Resources;

namespace Surreal.Graphics.Images;

public class ImageLoaderTests
{
  [Test]
  [TestCase("Assets/External/images/test.png")]
  public async Task it_should_load_an_image(string path)
  {
    using var manager = new ResourceManager();

    manager.AddLoader(new ImageLoader());

    var image = await manager.LoadResourceAsync<Image>(path);

    image.Width.Should().BeGreaterThan(0);
    image.Height.Should().BeGreaterThan(0);
    image.Size.Bytes.Should().BeGreaterThan(0);
  }
}

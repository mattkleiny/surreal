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

    Assert.That(image.Width, Is.GreaterThan(0));
    Assert.That(image.Height, Is.GreaterThan(0));
    Assert.That(image.Size.Bytes, Is.GreaterThan(0));
  }
}

using System.Threading.Tasks;
using NUnit.Framework;
using Surreal.Mathematics;

namespace Surreal.Graphics.Textures
{
  public sealed class ImageTests
  {
    [Test]
    public void it_should_create_a_mutable_image()
    {
      var image = new Image(256, 144);

      foreach (ref var pixel in image.Pixels)
      {
        pixel = Color.Yellow;
      }
    }

    [TestCase("local:Assets/Images/test.png")]
    [TestCase("local:Assets/Images/test.jpg")]
    [TestCase("local:Assets/Images/test.gif")]
    [TestCase("local:Assets/Images/test.tga")]
    public async Task it_should_load_different_image_formats(string path)
    {
      Assert.IsNotNull(await Image.LoadAsync(path));
    }
  }
}

namespace Surreal.Graphics.Images;

public class ImageTests
{
  [Test]
  public void it_should_calculate_size_correctly()
  {
    using var image = new Image(16, 16);

    image.Size.Bytes.Should().Be(16 * 16 * 4);
  }
}

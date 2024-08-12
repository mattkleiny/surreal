using Surreal.IO;

namespace Surreal.Graphics.Sprites.Aseprite;

public class AsepriteFileTests
{
  [Test]
  [TestCase("Assets/External/sprites/crab.aseprite")]
  public void it_should_load_a_simple_aseprite_file(VirtualPath path)
  {
    using var stream = path.OpenInputStream();

    var file = AsepriteFile.Load(stream);

    file.Should().NotBeNull();
  }
}

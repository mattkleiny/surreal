using Surreal.IO;

namespace Surreal.Mathematics;

public class SeedTests
{
  [Test]
  public void it_should_serialize_to_binary()
  {
    var memory = Seed.Randomized.ToByteArray();

    memory.Length.Should().Be(4);
  }
}

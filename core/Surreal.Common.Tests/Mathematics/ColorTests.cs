using System.Runtime.InteropServices;

namespace Surreal.Mathematics;

public class ColorTests
{
  [Test]
  public void it_should_layout_in_memory_correctly()
  {
    var pink = new Color32(255, 0, 255);
    var bytes = MemoryMarshal.AsBytes(stackalloc[] { pink });

    bytes[0].Should().Be(255);
    bytes[1].Should().Be(0);
    bytes[2].Should().Be(255);
    bytes[3].Should().Be(255);
  }
}

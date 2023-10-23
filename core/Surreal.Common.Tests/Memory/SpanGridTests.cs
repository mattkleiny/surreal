using Surreal.Memory;

namespace Surreal.Common.Tests.Memory;

public class SpanGridTests
{
  [Test]
  public void it_should_read_and_write_to_grid()
  {
    var grid = new SpanGrid<ushort>(stackalloc ushort[256], 16);

    grid.Width.Should().Be(16);
    grid.Height.Should().Be(16);

    for (var y = 0; y < grid.Height; y++)
    for (var x = 0; x < grid.Width; x++)
    {
      grid[x, y] = 100;
    }
  }
}

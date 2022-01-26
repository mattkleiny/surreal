namespace Surreal.Memory;

public class SpanGridTests
{
  [Test]
  public void it_should_read_and_write_to_grid()
  {
    var grid = new SpanGrid<ushort>(stackalloc ushort[256], 16);

    Assert.AreEqual(16, grid.Width);
    Assert.AreEqual(16, grid.Height);

    for (int y = 0; y < grid.Height; y++)
    for (int x = 0; x < grid.Width; x++)
    {
      grid[x, y] = 100;
    }
  }
}

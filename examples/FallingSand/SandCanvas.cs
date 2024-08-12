using Surreal.Graphics.Utilities;
using Surreal.Mathematics;

namespace FallingSand;

/// <summary>
/// A simple canvas of 'sand' pixels that can be simulated.
/// </summary>
public sealed class SandCanvas(IGraphicsBackend backend, int width = 256, int height = 144) : PixelCanvas(backend, width, height)
{
  private IntervalTimer _updateTimer = new(TimeSpan.FromMilliseconds(16));

  public void Update(DeltaTime deltaTime)
  {
    if (_updateTimer.Tick(deltaTime))
    {
      Simulate();

      _updateTimer.Reset();
    }
  }

  public void DrawSand(Point2 position, int radius, Color32 color)
  {
    var sqrRadius = radius * radius;
    var pixels = Pixels;

    for (int y = position.Y - radius; y < position.Y + radius; y++)
    for (int x = position.X - radius; x < position.X + radius; x++)
    {
      if (x < 0 || x > pixels.Width - 1) continue;
      if (y < 0 || y > pixels.Height - 1) continue;

      var xDiff = x - position.X;
      var yDiff = y - position.Y;

      if (xDiff * xDiff + yDiff * yDiff <= sqrRadius)
      {
        pixels[x, y] = color;
      }
    }
  }

  private void Simulate()
  {
    var pixels = Pixels;

    for (int y = pixels.Height - 1; y >= 0; y--)
    for (int x = 0; x < pixels.Width; x++)
    {
      ref var pixel = ref pixels[x, y];

      if (pixel != Color32.Clear)
      {
        if (SimulateSand(ref pixel, x, y + 1)) continue;
        if (SimulateSand(ref pixel, x - 1, y + 1)) continue;
        SimulateSand(ref pixel, x + 1, y + 1);
      }
    }
  }

  private bool SimulateSand(ref Color32 pixel, int x, int y)
  {
    if (x < 0 || x > Pixels.Width - 1) return false;
    if (y < 0 || y > Pixels.Height - 1) return false;

    ref var target = ref Pixels[x, y];

    if (pixel != Color32.Clear && target == Color32.Clear)
    {
      target = pixel;
      pixel = Color32.Clear;

      return true;
    }

    return false;
  }
}

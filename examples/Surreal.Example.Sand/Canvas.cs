namespace Surreal;

/// <summary>A simple canvas of 'sand' pixels that can be simulated.</summary>
public sealed class Canvas : PixelCanvas
{
  private static readonly Color32 Empty = Color32.White;

  private IntervalTimer updateTimer = new(16.Milliseconds());

  public Canvas(IGraphicsServer server, int width, int height)
    : base(server, width, height)
  {
    Pixels.Fill(Empty);
  }

  public void AddSand(Point2 position, int radius, Color32 color)
  {
    Pixels.DrawCircle(position, radius, color);
  }

  public void DeleteSand(Point2 position, int radius)
  {
    Pixels.DrawCircle(position, radius, Empty);
  }

  public void Update(TimeDelta deltaTime)
  {
    if (updateTimer.Tick(deltaTime))
    {
      Simulate();

      updateTimer.Reset();
    }
  }

  private void Simulate()
  {
    var span = Pixels;

    for (int y = span.Height - 1; y >= 0; y--)
    for (int x = 0; x < span.Width; x++)
    {
      ref var pixel = ref span[x, y];

      if (pixel != Empty)
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

    if (pixel != Empty && target == Empty)
    {
      target = pixel;
      pixel  = Empty;

      return true;
    }

    return false;
  }

  public void Clear()
  {
    Pixels.Fill(Empty);
  }
}

using Surreal.Pixels;

namespace Asteroids.Actors;

/// <summary>A <see cref="PixelCanvas"/> for the asteroids example.</summary>
public sealed class AsteroidsCanvas : PixelCanvas
{
  private IntervalTimer updateTimer = new(16.Milliseconds());

  public AsteroidsCanvas(IGraphicsServer server, int width, int height)
    : base(server, width, height)
  {
  }

  public bool IsExploding { get; set; }

  public void Update(TimeDelta deltaTime)
  {
    var pixels = Span;

    if (!IsExploding)
    {
      pixels.Fill(Color32.Black);
      return;
    }

    if (updateTimer.Tick(deltaTime))
    {
      for (int y = pixels.Height - 1; y >= 0; y--)
      for (int x = 0; x < pixels.Width; x++)
      {
        ref var pixel = ref pixels[x, y];

        if (pixel != Color32.Black)
        {
          if (SimulateSand(ref pixel, x, y + 1)) continue;
          if (SimulateSand(ref pixel, x - 1, y + 1)) continue;
          SimulateSand(ref pixel, x + 1, y + 1);
        }
      }

      updateTimer.Reset();
    }
  }

  private bool SimulateSand(ref Color32 pixel, int x, int y)
  {
    if (x < 0 || x > Width - 1) return false;
    if (y < 0 || y > Height - 1) return false;

    ref var target = ref Span[x, y];

    if (target == Color32.Black)
    {
      target = pixel;
      pixel  = Color32.Black;

      return true;
    }

    return false;
  }
}

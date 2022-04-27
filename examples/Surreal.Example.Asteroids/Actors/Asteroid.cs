using Surreal.Actors;
using Surreal.Pixels;

namespace Asteroids.Actors;

public sealed class Asteroid : Actor
{
  private readonly PixelCanvas canvas;

  public Asteroid(PixelCanvas canvas)
  {
    this.canvas = canvas;
  }

  public Polygon Polygon  { get; set; } = Polygon.Create();
  public Vector2 Velocity { get; set; } = Vector2.Zero;
  public float   Rotation { get; set; } = 0f;
  public Color   Color    { get; set; } = Color.White;

  public Vector2 Position
  {
    get => Polygon.Center;
    set => Polygon.Translate(value);
  }

  protected override void OnUpdate(TimeDelta deltaTime)
  {
    base.OnUpdate(deltaTime);

    // Polygon.Rotate(Rotation * deltaTime);
    Polygon.Translate(Velocity * deltaTime);
  }

  protected override void OnDraw(TimeDelta time)
  {
    base.OnDraw(time);

    var pixels = canvas.Span;
    var box = Box.Create(Polygon.Center, Polygon.Size).Clamp(0, 0, pixels.Width - 1, pixels.Height - 1);

    for (int y = box.Bottom; y < box.Top; y++)
    for (int x = box.Left; x < box.Right; x++)
    {
      var point = new Vector2(x, y);

      if (Polygon.ContainsPoint(point))
      {
        pixels[x, y] = Color;
      }
      else
      {
        pixels[x, y] = Color.Magenta with { A = 0.2f };
      }
    }
  }
}

using Surreal.Actors;
using Surreal.Pixels;

namespace Asteroids.Actors;

/// <summary>An asteroid that floats about and can impact the player.</summary>
public sealed class Asteroid : Actor
{
  private readonly PixelCanvas canvas;
  private readonly Polygon transformedPolygon;

  public Asteroid(PixelCanvas canvas)
  {
    this.canvas = canvas;

    transformedPolygon = new Polygon(new Vector2[Polygon.Length]);
  }

  public Polygon Polygon  { get; init; } = Polygon.Create();
  public Vector2 Position { get; set; }  = Vector2.Zero;
  public Vector2 Velocity { get; set; }  = Vector2.Zero;
  public float   Rotation { get; set; }  = 0f;
  public float   Spin     { get; set; }  = 0f;
  public Color   Color    { get; set; }  = Color.White;

  protected override void OnUpdate(TimeDelta deltaTime)
  {
    base.OnUpdate(deltaTime);

    Position += Velocity * deltaTime;
    Rotation += Spin * deltaTime;

    var rotation = Matrix4x4.CreateRotationZ(Rotation);
    var translation = Matrix4x4.CreateTranslation(Position.X, Position.Y, 0f);

    var transform = rotation * translation;

    transformedPolygon.TransformFrom(Polygon, in transform);
  }

  protected override void OnDraw(TimeDelta time)
  {
    base.OnDraw(time);

    var pixels = canvas.Span;
    var rectangle = transformedPolygon.Bounds.Clamp(0, 0, pixels.Width - 1, pixels.Height - 1);

    for (int y = (int) rectangle.Bottom; y < (int) rectangle.Top; y++)
    for (int x = (int) rectangle.Left; x < (int) rectangle.Right; x++)
    {
      var point = new Vector2(x, y);

      if (transformedPolygon.ContainsPoint(point))
      {
        pixels[x, y] = Color;
      }
    }
  }
}

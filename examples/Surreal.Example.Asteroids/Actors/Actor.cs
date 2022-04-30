namespace Asteroids.Actors;

/// <summary>An <see cref="Surreal.Actors.Actor"/> that maintains a polygonal shape.</summary>
public abstract class Actor : Surreal.Actors.Actor
{
  private readonly PixelCanvas canvas;

  protected Actor(PixelCanvas canvas, Polygon polygon)
  {
    this.canvas   = canvas;
    SourcePolygon = polygon;
  }

  public Vector2 Position = Vector2.Zero;
  public Vector2 Velocity = Vector2.Zero;
  public float Rotation = 0f;
  public float Spin = 0f;
  public Color32 Color = Color32.White;

  public Polygon SourcePolygon { get; }
  public Polygon FinalPolygon  { get; } = new();

  /// <summary>The final bounds of the actor's polygon, constrained to the canvas.</summary>
  public BoundingRect Bounds => FinalPolygon.Bounds.Clamp(0, 0, canvas.Width - 1, canvas.Height - 1);

  protected override void OnUpdate(TimeDelta deltaTime)
  {
    base.OnUpdate(deltaTime);

    CheckIfOffScreen();
    UpdatePositionAndRotation(deltaTime);
  }

  protected override void OnDraw(TimeDelta deltaTime)
  {
    base.OnDraw(deltaTime);

    DrawPolygon();
  }

  private void UpdatePositionAndRotation(TimeDelta deltaTime)
  {
    // update position and rotation
    Position += Velocity * deltaTime;
    Rotation += Spin * deltaTime;

    var rotation = Matrix4x4.CreateRotationZ(Rotation);
    var translation = Matrix4x4.CreateTranslation(Position.X, Position.Y, 0f);

    var transform = rotation * translation;

    FinalPolygon.TransformFrom(SourcePolygon, in transform);
  }

  /// <summary>Checks to see if we've gone off-screen and moves us back if we have.</summary>
  private void CheckIfOffScreen()
  {
    // are we off-screen?
    var halfSize = SourcePolygon.Size / 2f;

    if (Velocity.X < 0f && Position.X + halfSize.X < 0)
    {
      Position.X = canvas.Width + halfSize.X; // left
    }

    if (Velocity.Y < 0f && Position.Y + halfSize.Y < 0)
    {
      Position.Y = canvas.Height + halfSize.Y; // top
    }

    if (Velocity.X > 0f && Position.X - halfSize.X > canvas.Width)
    {
      Position.X = -halfSize.X; // right
    }

    if (Velocity.Y > 0f && Position.Y - halfSize.Y > canvas.Height)
    {
      Position.Y = -halfSize.Y; // bottom
    }
  }

  private void DrawPolygon()
  {
    var pixels = canvas.Pixels;
    var rectangle = Bounds;

    for (int y = (int) rectangle.Bottom; y < (int) rectangle.Top; y++)
    for (int x = (int) rectangle.Left; x < (int) rectangle.Right; x++)
    {
      var point = new Vector2(x, y);

      if (FinalPolygon.ContainsPoint(point))
      {
        pixels[x, y] = Color;
      }
    }
  }
}

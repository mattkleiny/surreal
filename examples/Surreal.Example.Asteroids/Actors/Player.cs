using Surreal.Actors;
using Surreal.Memory;
using Surreal.Pixels;

namespace Asteroids.Actors;

public sealed class Player : Actor
{
  private readonly PixelCanvas canvas;
  private readonly IKeyboardDevice keyboard;

  public Player(PixelCanvas canvas, IKeyboardDevice keyboard)
  {
    this.canvas   = canvas;
    this.keyboard = keyboard;
  }

  public Vector2 Position { get; set; } = Vector2.Zero;
  public Color   Color    { get; set; } = Color.White;
  public float   Speed    { get; set; } = 100f;

  protected override void OnInput(TimeDelta deltaTime)
  {
    base.OnInput(deltaTime);

    var direction = Vector2.Zero;

    if (keyboard.IsKeyDown(Key.W)) direction.Y -= Speed * deltaTime;
    if (keyboard.IsKeyDown(Key.S)) direction.Y += Speed * deltaTime;
    if (keyboard.IsKeyDown(Key.A)) direction.X -= Speed * deltaTime;
    if (keyboard.IsKeyDown(Key.D)) direction.X += Speed * deltaTime;

    Position += direction;
  }

  protected override void OnDraw(TimeDelta time)
  {
    base.OnDraw(time);

    var size = new Point2(4, 4);

    canvas.Span.DrawRectangle(Position, size, Color);
  }
}

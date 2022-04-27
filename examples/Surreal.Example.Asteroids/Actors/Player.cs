using Surreal.Actors;
using Surreal.Memory;
using Surreal.Pixels;

namespace Asteroids.Actors;

/// <summary>The player ship.</summary>
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

    var movement = Vector2.Zero;

    if (keyboard.IsKeyDown(Key.W)) movement.Y -= Speed;
    if (keyboard.IsKeyDown(Key.S)) movement.Y += Speed;
    if (keyboard.IsKeyDown(Key.A)) movement.X -= Speed;
    if (keyboard.IsKeyDown(Key.D)) movement.X += Speed;

    if (movement.LengthSquared() > 0f)
    {
      Position += movement * deltaTime;
    }
  }

  protected override void OnDraw(TimeDelta time)
  {
    base.OnDraw(time);

    canvas.Span.DrawCircle(Position, 6, Color with { A = 0.2f });
  }
}

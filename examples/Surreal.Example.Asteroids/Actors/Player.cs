namespace Asteroids.Actors;

/// <summary>The player ship.</summary>
public sealed class Player : Actor
{
  private readonly Canvas canvas;
  private readonly IKeyboardDevice keyboard;
  private readonly ActorScene scene;

  public Player(Canvas canvas, IKeyboardDevice keyboard, ActorScene scene)
    : base(canvas, Polygon.CreateTriangle(4f))
  {
    this.canvas   = canvas;
    this.keyboard = keyboard;
    this.scene    = scene;
  }

  public float Speed { get; set; } = 50f;

  protected override void OnInput(TimeDelta deltaTime)
  {
    base.OnInput(deltaTime);

    HandleKeyboardInput();
  }

  public void OnHitAsteroid(Asteroid asteroid)
  {
    canvas.IsGameOver = true;

    scene.Clear();
  }

  private void HandleKeyboardInput()
  {
    var movement = Vector2.Zero;
    var spin = 0f;

    if (keyboard.IsKeyDown(Key.W)) movement.Y -= Speed;
    if (keyboard.IsKeyDown(Key.S)) movement.Y += Speed;
    if (keyboard.IsKeyDown(Key.A)) spin       -= 5.0f;
    if (keyboard.IsKeyDown(Key.D)) spin       += 5.0f;

    var rotation = Matrix3x2.CreateRotation(Rotation);

    Velocity = Vector2.Transform(movement, rotation);
    Spin     = spin;
  }
}

namespace Surreal.Actors;

/// <summary>The player ship.</summary>
public sealed class Player : GameActor
{
  private readonly Canvas _canvas;
  private readonly IKeyboardDevice _keyboard;
  private readonly ActorScene _scene;

  public Player(Canvas canvas, IKeyboardDevice keyboard, ActorScene scene)
    : base(canvas, Polygon.CreateTriangle(4f))
  {
    _canvas = canvas;
    _keyboard = keyboard;
    _scene = scene;
  }

  public float Speed { get; set; } = 50f;
  public Color32 ProjectileColor { get; set; } = Color32.Red;

  protected override void OnInput(TimeDelta deltaTime)
  {
    base.OnInput(deltaTime);

    HandleKeyboardInput();
  }

  private void HandleKeyboardInput()
  {
    var movement = Vector2.Zero;
    var spin = 0f;

    if (_keyboard.IsKeyDown(Key.W))
    {
      movement.Y -= Speed;
    }

    if (_keyboard.IsKeyDown(Key.S))
    {
      movement.Y += Speed;
    }

    if (_keyboard.IsKeyDown(Key.A))
    {
      spin -= 5.0f;
    }

    if (_keyboard.IsKeyDown(Key.D))
    {
      spin += 5.0f;
    }

    var rotation = Matrix3x2.CreateRotation(Rotation);
    var forward = Vector2.Transform(movement, rotation);

    Velocity = forward;
    Spin = spin;

    if (_keyboard.IsKeyPressed(Key.Space))
    {
      Spawn(new Projectile(_canvas, _scene)
      {
        Position = Position,
        Rotation = Rotation,
        Color = ProjectileColor
      });
    }
  }

  [MessageSubscriber]
  private void OnPlayerHitAsteroid(ref PlayerHitAsteroid message)
  {
    _canvas.IsGameOver = true;

    _scene.Clear();
  }
}



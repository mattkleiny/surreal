namespace Asteroids.Actors;

/// <summary>The player ship.</summary>
public sealed class Player : PolygonActor
{
  private readonly AsteroidsCanvas canvas;
  private readonly IKeyboardDevice keyboard;
  private readonly ActorScene scene;

  public Player(AsteroidsCanvas canvas, IKeyboardDevice keyboard, ActorScene scene)
    : base(canvas, CreatePlayerPolygon(4f))
  {
    this.canvas   = canvas;
    this.keyboard = keyboard;
    this.scene    = scene;
  }

  public float Speed { get; set; } = 50f;

  protected override void OnInput(TimeDelta deltaTime)
  {
    base.OnInput(deltaTime);

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

  public void OnHitAsteroid(Asteroid asteroid)
  {
    canvas.IsGameOver = true;

    scene.Clear();
  }

  /// <summary>Creates a new randomly shaped <see cref="Polygon"/> to represent a player.</summary>
  private static Polygon CreatePlayerPolygon(float scale)
  {
    var vertices = new Vector2[3];

    vertices[0] = new Vector2(-scale, scale);
    vertices[1] = new Vector2(0f, -scale);
    vertices[2] = new Vector2(scale, scale);

    return new Polygon(vertices);
  }
}

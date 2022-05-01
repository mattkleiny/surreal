namespace Asteroids.Actors;

/// <summary>A projectile that can destroy asteroids.</summary>
public sealed class Projectile : GameActor
{
  private readonly ActorScene scene;

  public Projectile(PixelCanvas canvas, ActorScene scene)
    : base(canvas, Polygon.CreateTriangle(2f))
  {
    this.scene = scene;
  }

  public float Speed { get; set; } = 100f;

  protected override void OnUpdate(TimeDelta deltaTime)
  {
    base.OnUpdate(deltaTime);

    UpdateMovement();
    ProcessCollisions();
  }

  private void UpdateMovement()
  {
    var rotation = Matrix3x2.CreateRotation(Rotation);
    var forward = Vector2.UnitY * Speed;

    Velocity = -Vector2.Transform(forward, rotation);
  }

  private void ProcessCollisions()
  {
    foreach (var actor in scene)
    {
      if (actor is Asteroid asteroid && asteroid.Bounds.Contains(Position) && asteroid.FinalPolygon.ContainsPoint(Position))
      {
        Message.Publish(new ProjectileHitAsteroid(this, asteroid));
      }
    }
  }

  [MessageSubscriber]
  private void OnProjectileHitAsteroid(ref ProjectileHitAsteroid message)
  {
    if (message.Projectile == this)
    {
      Destroy();
    }
  }
}

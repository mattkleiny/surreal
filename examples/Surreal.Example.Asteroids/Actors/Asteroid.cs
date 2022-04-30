namespace Asteroids.Actors;

/// <summary>An asteroid that floats about and can impact the player.</summary>
public sealed class Asteroid : GameActor
{
  private readonly Player player;

  public Asteroid(PixelCanvas canvas, Player player)
    : base(canvas, Polygon.CreateAsteroid(new FloatRange(4f, 12f)))
  {
    this.player = player;
  }

  protected override void OnUpdate(TimeDelta deltaTime)
  {
    base.OnUpdate(deltaTime);

    CheckForCollisions();
  }

  private void CheckForCollisions()
  {
    if (Bounds.Contains(player.Position) && FinalPolygon.ContainsPoint(player.Position))
    {
      Message.Publish(new PlayerHitAsteroid(player, this));
    }
  }

  [MessageSubscriber]
  private void OnProjectileHitAsteroid(ref ProjectileHitAsteroid message)
  {
    if (message.Asteroid == this)
    {
      // TODO: break into smaller pieces?

      Destroy();
    }
  }
}

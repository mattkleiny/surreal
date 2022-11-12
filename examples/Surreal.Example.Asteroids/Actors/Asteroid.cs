namespace Surreal.Actors;

/// <summary>An asteroid that floats about and can impact the player.</summary>
public sealed class Asteroid : GameActor
{
  private readonly Player _player;

  public Asteroid(PixelCanvas canvas, Player player)
    : base(canvas, Polygon.CreateAsteroid(new FloatRange(4f, 12f)))
  {
    _player = player;
  }

  protected override void OnUpdate(TimeDelta deltaTime)
  {
    base.OnUpdate(deltaTime);

    CheckForCollisions();
  }

  private void CheckForCollisions()
  {
    if (Bounds.Contains(_player.Position) && FinalPolygon.ContainsPoint(_player.Position))
    {
      Message.Publish(new PlayerHitAsteroid(_player, this));
    }
  }

  [MessageSubscriber]
  private void OnProjectileHitAsteroid(ref ProjectileHitAsteroid message)
  {
    if (message.Asteroid == this)
    {
      Destroy();
    }
  }
}



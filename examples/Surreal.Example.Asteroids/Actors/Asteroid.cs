namespace Asteroids.Actors;

/// <summary>An asteroid that floats about and can impact the player.</summary>
public sealed class Asteroid : PolygonActor
{
  private readonly Player player;

  public Asteroid(PixelCanvas canvas, Player player)
    : base(canvas, CreateAsteroidPolygon(new FloatRange(4f, 12f)))
  {
    this.player = player;
  }

  protected override void OnUpdate(TimeDelta deltaTime)
  {
    base.OnUpdate(deltaTime);

    if (Bounds.Contains(player.Position) && FinalPolygon.ContainsPoint(player.Position))
    {
      player.OnHitAsteroid(this);
    }
  }

  /// <summary>Creates a new randomly shaped <see cref="Polygon"/> to represent an asteroid.</summary>
  private static Polygon CreateAsteroidPolygon(FloatRange radiusRange)
  {
    var random = Random.Shared;

    var vertices = new Vector2[random.NextInt(5, 12)];
    var list = new SpanList<Vector2>(vertices);

    var theta = 0f;

    for (var i = 0; i < list.Capacity; i++)
    {
      theta += 2 * MathF.PI / list.Capacity;

      var radius = random.NextRange(radiusRange);

      var x = radius * MathF.Cos(theta);
      var y = radius * MathF.Sin(theta);

      list.Add(new Vector2(x, y));
    }

    return new Polygon(vertices);
  }
}

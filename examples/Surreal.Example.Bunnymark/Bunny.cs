namespace Surreal;

public sealed class Bunny
{
  private readonly Texture sprite;
  private readonly SpriteBatch batch;

  public Bunny(Texture sprite, SpriteBatch batch)
  {
    this.sprite = sprite;
    this.batch  = batch;

    Velocity = Random.Shared.NextUnitCircle();
  }

  public Vector2 Position { get; set; }
  public Vector2 Velocity { get; set; }

  public void Update()
  {
    Position += Velocity;
  }

  public void Draw()
  {
    batch.Draw(sprite, Position);
  }
}

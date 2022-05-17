namespace Surreal;

public record struct Bunny
{
  private readonly Texture sprite;
  private readonly SpriteBatch batch;

  public Bunny(Texture sprite, SpriteBatch batch)
  {
    this.sprite = sprite;
    this.batch  = batch;

    Position = Vector2.Zero;
    Velocity = Random.Shared.NextUnitCircle();
  }

  public Vector2 Position;
  public Vector2 Velocity;

  public void Update()
  {
    Position += Velocity;
  }

  public void Draw()
  {
    batch.Draw(sprite, Position);
  }
}

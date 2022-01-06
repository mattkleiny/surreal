using Surreal.Graphics.Textures;

namespace Asteroids.Actors;

public sealed class Ship : Actor
{
  public Ship(TextureRegion sprite)
  {
    AddComponent(new Health(1));
    AddComponent(new Sprite(sprite));
  }

  public ref Health Health => ref GetComponent<Health>();
  public ref Sprite Sprite => ref GetComponent<Sprite>();
}

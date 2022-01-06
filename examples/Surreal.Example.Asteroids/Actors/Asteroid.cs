using Surreal.Graphics.Textures;

namespace Asteroids.Actors;

public sealed class Asteroid : Actor
{
  public Asteroid(IActorContext context, TextureRegion sprite)
    : base(context)
  {
    AddComponent(new Health(1));
    AddComponent(new Sprite(sprite));
  }

  public ref Health Health => ref GetComponent<Health>();
  public ref Sprite Sprite => ref GetComponent<Sprite>();
}

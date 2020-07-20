using Surreal.Graphics.Textures;

namespace Asteroids.Actors {
  public sealed class Asteroid : Actor {
    public Asteroid(TextureRegion sprite)
        : base(sprite) {
    }

    public int Health { get; set; } = 10;
  }
}
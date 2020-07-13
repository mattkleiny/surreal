using Asteroids.Actors.Components;
using Surreal.Graphics.Textures;

namespace Asteroids.Actors {
  public sealed class Asteroid : AsteroidActor {
    public int Health { get; set; } = 10;

    public Asteroid(TextureRegion sprite) {
      Components.Add(new SpriteComponent(this) {
          Sprite = sprite
      });
    }
  }
}
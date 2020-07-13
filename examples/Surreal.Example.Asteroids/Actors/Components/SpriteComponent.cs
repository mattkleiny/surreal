using Surreal.Framework.Scenes.Actors;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Timing;

namespace Asteroids.Actors.Components {
  public sealed class SpriteComponent : ActorComponent<AsteroidActor> {
    public TextureRegion? Sprite { get; set; }

    public SpriteComponent(AsteroidActor actor)
        : base(actor) {
    }

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      if (Sprite != null) {
        AsteroidsGame.Current.SpriteBatch.DrawPivoted(
            region: Sprite,
            position: Actor.Position,
            pivot: Pivot.Center,
            rotation: Actor.Rotation
        );
      }
    }
  }
}
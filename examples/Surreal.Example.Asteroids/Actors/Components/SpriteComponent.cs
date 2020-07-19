using Surreal.Framework.Scenes.Actors;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Mathematics.Timing;

namespace Asteroids.Actors.Components {
  public sealed class SpriteComponent : ActorComponent<Actor> {
    public SpriteComponent(TextureRegion sprite) {
      Sprite = sprite;
    }

    public TextureRegion Sprite { get; set; }

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      Game.Current.SpriteBatch.DrawPivoted(
          region: Sprite,
          position: Actor!.Position,
          pivot: Pivot.Center,
          rotation: Angle.Zero
      );
    }
  }
}
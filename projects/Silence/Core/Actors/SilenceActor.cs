using Silence.Core.Sprites;
using Surreal.Framework.Scenes.Actors;
using Surreal.Timing;

namespace Silence.Core.Actors {
  public class SilenceActor : Actor2D {
    public SilenceGame    Game   { get; }
    public GeometrySprite Sprite { get; set; }

    public SilenceActor(SilenceGame game) {
      Game = game;
    }

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      Sprite.Draw(Game.GeometryBatch);
    }
  }
}
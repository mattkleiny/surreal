using System.Numerics;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Mathematics.Timing;

namespace Asteroids.Actors {
  public abstract class Actor : Surreal.Framework.Scenes.Actors.Actor {
    public Vector2       Position { get; set; } = Vector2.Zero;
    public TextureRegion Sprite   { get; }

    protected Actor(TextureRegion sprite) {
      Sprite = sprite;
    }

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      Game.Current.SpriteBatch.DrawPivoted(
          region: Sprite,
          position: Position,
          pivot: Pivot.Center,
          rotation: Angle.Zero
      );
    }

    protected override void ComputeModelToWorld(out Matrix4x4 modelToWorld) {
      modelToWorld = Matrix4x4.CreateTranslation(Position.X, Position.Y, 0f);
    }
  }
}
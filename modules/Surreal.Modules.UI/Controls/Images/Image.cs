using System;
using System.Numerics;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Mathematics.Linear;
using Surreal.Mathematics.Timing;

namespace Surreal.UI.Controls.Images {
  public class Image : Control {
    public TextureRegion Sprite   { get; }
    public Vector2       Position { get; set; } = Vector2.Zero;
    public Vector2       Scale    { get; set; } = Vector2.One;

    public Image(TextureRegion sprite) {
      Sprite = sprite;
    }

    protected override Rectangle ComputeLayout() {
      throw new NotImplementedException();
    }

    public override void Draw(DeltaTime deltaTime, SpriteBatch batch) {
      base.Draw(deltaTime, batch);

      batch.Draw(
          region: Sprite,
          x: Position.X,
          y: Position.Y,
          rotation: Angle.Zero,
          width: Sprite.Width * Scale.X,
          height: Sprite.Height * Scale.Y
      );
    }
  }
}
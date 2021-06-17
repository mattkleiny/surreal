using System;
using System.Numerics;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Mathematics.Linear;
using Surreal.Mathematics.Timing;

namespace Surreal.UI.Controls {
  public class Button : Control {
    public TextureRegion Texture  { get; }
    public Vector2       Scale    { get; }
    public Vector2       Position { get; }

    public Button(TextureRegion texture, Vector2 position, Vector2 scale) {
      Texture  = texture;
      Scale    = scale;
      Position = position;
    }

    protected override Rectangle ComputeLayout() {
      throw new NotImplementedException();
    }

    public override void Draw(DeltaTime deltaTime, SpriteBatch batch) {
      base.Draw(deltaTime, batch);

      batch.Draw(
          region: Texture,
          x: Position.X,
          y: Position.Y,
          rotation: Angle.Zero,
          width: Texture.Width * Scale.X,
          height: Texture.Height * Scale.Y
      );
    }
  }
}
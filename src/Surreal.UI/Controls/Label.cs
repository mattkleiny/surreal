using System;
using System.Numerics;
using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Sprites;
using Surreal.Mathematics.Linear;
using Surreal.Timing;

namespace Surreal.UI.Controls
{
  public class Label : Control
  {
    public BitmapFont Font     { get; set; }
    public string     Text     { get; set; } = string.Empty;
    public Vector2    Position { get; set; } = Vector2.Zero;
    public Color      Color    { get; set; } = Color.White;
    public float      Scale    { get; set; } = 1f;

    public Label(BitmapFont font, string text)
    {
      Font = font;
      Text = text;
    }

    protected override Rectangle ComputeLayout()
    {
      throw new NotImplementedException();
    }

    public override void Draw(DeltaTime deltaTime, SpriteBatch batch)
    {
      base.Draw(deltaTime, batch);

      // TODO: work out how to lay this guy out relative to it's parent 

      batch.DrawText(
        text: Text,
        font: Font,
        position: Position,
        color: Color,
        scale: Scale
      );
    }
  }
}
namespace Surreal.Graphics.UI.Editors;

/// <summary>The <see cref="ImmediateModeEditor{T}"/> for <see cref="Vector2"/>.</summary>
[ImmediateModeEditor(typeof(Vector2))]
public sealed class Vector2Editor : ImmediateModeEditor<Vector2>
{
  public override void DrawReadOnly(IImmediateModeContext layout, Vector2 value)
  {
    layout.BeginHorizontal();

    layout.DrawText($"X: {value.X}");
    layout.DrawText($"Y: {value.Y}");

    layout.EndHorizontal();
  }

  public override void DrawReadWrite(IImmediateModeContext layout, ref Vector2 value)
  {
    layout.BeginHorizontal();

    layout.DrawText("X:");
    layout.DrawEditor(ref value.X);

    layout.DrawText("Y:");
    layout.DrawEditor(ref value.X);

    layout.EndHorizontal();
  }
}

using Surreal.Mathematics;

namespace Surreal.UI.Controls;

/// <summary>Methods for rendering in an <see cref="ImmediateModeContext"/>.</summary>
public static class ButtonControls
{
  public static bool Button(this ImmediateModeContext context, Rectangle rectangle, string label)
  {
    return Button(context, rectangle, label, context.Skin.DefaultStyle);
  }

  public static bool Button(this ImmediateModeContext context, Rectangle rectangle, string label, Style style)
  {
    throw new NotImplementedException();
  }
}

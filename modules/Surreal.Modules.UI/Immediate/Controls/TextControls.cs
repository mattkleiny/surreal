using Surreal.Mathematics;

namespace Surreal.UI.Immediate.Controls;

/// <summary>Text controls for immediate mode UI rendering.</summary>
public static class TextControls
{
  public static string TextField(this ImmediateModeContext context, Rectangle rectangle, string text)
  {
    return TextField(context, rectangle, text, context.Skin.DefaultStyle);
  }

  public static string TextField(this ImmediateModeContext context, Rectangle rectangle, string text, Style style)
  {
    throw new NotImplementedException();
  }

  /// <summary>A state object used for text field rendering.</summary>
  private sealed record TextFieldState
  {
  }
}

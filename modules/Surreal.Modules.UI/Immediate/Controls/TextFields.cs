using Surreal.Mathematics;

namespace Surreal.UI.Immediate.Controls;

/// <summary>Text controls for immediate mode UI rendering.</summary>
public static class TextFields
{
  public static StyleKey TextFieldStyle { get; } = new("textfield");

  public static string TextField(this ImmediateModeContext context, Rectangle rectangle, string text)
  {
    return TextField(context, rectangle, text, context.Skin.GetStyleOrDefault(TextFieldStyle));
  }

  public static string TextField(this ImmediateModeContext context, Rectangle rectangle, string text, Style style)
  {
    var id = context.GetControlId(rectangle);
    var state = context.GetState<TextFieldState>(id);

    throw new NotImplementedException();
  }

  private sealed record TextFieldState;
}
